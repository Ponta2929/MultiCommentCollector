using MCC.Utility;
using MCC.Utility.IO;
using MCC.Utility.Net;
using System.Threading.Tasks;

namespace MCC.Youtube
{
    internal class YoutubeConnector : ILogged
    {
        private string APIKey;
        private bool called;
        private string chatId;
        private string videoId;

        public bool Connected { get; set; }
        public bool Resume { get; set; }

        public event LoggedEventHandler OnLogged;

        public event ChatReceivedEventHandler OnReceived;

        public YoutubeConnector() { }

        public async void Connect(string apiKey, string streamKey)
        {
            APIKey = apiKey;

            while (Resume)
            {
                var feed = Http.Get($"https://www.youtube.com/feeds/videos.xml?channel_id={streamKey}");
                var deserialize = XmlSerializer.Deserialize<Feed>(feed);

                if (deserialize.Entry is not null && deserialize.Entry.Length > 0)
                {
                    if (IsLiving(deserialize.Entry[0].VideoID))
                    {
                        if (!Connected)
                        {
                            chatId = GetChatID(deserialize.Entry[0].VideoID);

                            Connected = true;
                            called = false;

                            Logged(LogLevel.Info, "接続を開始しました。");

                            await Task.Run(() => MessageLoop(chatId));
                        }
                    }
                    else
                    {
                        if (Connected)
                        {
                            Logged(LogLevel.Info, "切断されました。");
                        }

                        Connected = false;

                        if (!called)
                        {
                            called = true;
                            Logged(LogLevel.Info, "現在配信を行っていません。");
                        }
                    }
                }
                else
                {
                    if (!called)
                    {
                        called = true;
                        Logged(LogLevel.Info, "動画リストを取得できません。");
                    }
                }

                await Task.Delay(60000);
            }
        }

        private async void MessageLoop(string chatId)
        {
            var pageToken = string.Empty;

            while (Connected)
            {
                pageToken = GetChat(chatId, pageToken);

                // 15秒待つ
                await Task.Delay(15000);
            }
        }

        private bool IsLiving(string videoId)
        {
            var videos = Http.Get($"https://www.googleapis.com/youtube/v3/videos?key={APIKey}&id={videoId}&part=snippet");

            if (!string.IsNullOrEmpty(videos))
            {
                var response = System.Text.Json.JsonSerializer.Deserialize<VideoListResponse>(videos);


                foreach (var item in response.items)
                {
                    if (item.snippet.liveBroadcastContent.Equals("live"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string GetChatID(string videoId)
        {
            var videos = Http.Get($"https://www.googleapis.com/youtube/v3/videos?key={APIKey}&id={videoId}&part=liveStreamingDetails");
            var response = System.Text.Json.JsonSerializer.Deserialize<VideoListResponse>(videos);

            foreach (var item in response.items)
            {
                return item.liveStreamingDetails.activeLiveChatId ?? string.Empty;
            }

            return string.Empty;
        }

        private string GetChat(string chatId, string pageToken)
        {
            var postData = $"https://www.googleapis.com/youtube/v3/liveChat/messages?key={APIKey}&liveChatId={chatId}&part=id,snippet,authorDetails";

            if (!string.IsNullOrEmpty(pageToken))
            {
                postData += $"&pageToken={pageToken}";
            }

            var videos = Http.Get(postData);
            var response = System.Text.Json.JsonSerializer.Deserialize<VideoListResponse>(videos);

            OnReceived?.Invoke(this, new(response));

            return response.nextPageToken;
        }

        public void Abort() => Connected = false;

        private void Logged(LogLevel level, string message) => BaseLogged(this, new(level, message));

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}

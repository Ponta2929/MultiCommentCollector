using MCC.Plugin;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MCC.Youtube
{
    public class Youtube : IPluginSender, ISetting, ILogged
    {
        private Setting setting = Setting.Instance;
        private YoutubeConnector connector = new();
        private Hashtable table = new();

        public string SiteName => "Youtube";

        public string StreamKey { get; set; }

        public string Author => "ぽんた";

        public string PluginName => "Youtube";

        public string Description => "YoutubeLiveのコメントを取得します。";

        public string Version => "1.0.0.0";

        public string MenuItemName => "設定";

        public event CommentReceivedEventHandler OnCommentReceived;
        public event LoggedEventHandler OnLogged;

        public bool Activate()
        {
            connector.Resume = true;

            Task.Run(() => connector.Connect(setting.APIKey, StreamKey));

            return true;
        }
        public bool Inactivate()
        {
            connector.Resume = false;

            connector.Abort();

            return true;
        }

        public bool IsSupport(string url)
        {
            StreamKey = url.RegexString(@"https://www.youtube.com/channel/(?<value>[\w\-]+)", "value");

            if (!string.IsNullOrEmpty(StreamKey) && !string.IsNullOrEmpty(Http.Get($"https://www.youtube.com/feeds/videos.xml?channel_id={StreamKey}")))
            {
                return true;
            }

            return false;
        }

        public void PluginClose() => Inactivate();

        public void PluginLoad()
        {
            connector.OnLogged += BaseLogged;
            connector.OnReceived += OnReceived;

            table["Streamer"] = new AdditionalData() { Data = "Streamer", Description = "Streamer" };
            table["M"] = new AdditionalData() { Data = "M", Description = "モデレーター" };
            table["S"] = new AdditionalData() { Data = "S", Description = "スポンサー" };
        }

        private void OnReceived(object sender, ChatReceivedEventArgs e)
        {
            foreach (var item in e.ReceiveData.items)
            {
                var list = new List<AdditionalData>();

                var commentData = new CommentData()
                {
                    LiveName = "Youtube",
                    PostTime = item.snippet.publishedAt,
                    Comment = item.snippet.displayMessage,
                    UserName = item.authorDetails.displayName,
                    UserID = item.authorDetails.channelId
                };
                if (item.authorDetails.isChatOwner)
                {
                    list.Add(table["Streamer"] as AdditionalData);
                }

                if (item.authorDetails.isChatModerator)
                {
                    list.Add(table["M"] as AdditionalData);
                }

                if (item.authorDetails.isChatSponsor)
                {
                    list.Add(table["S"] as AdditionalData);
                }

                if (item.snippet.type.Equals("superChatEvent"))
                {
                    list.Add(new AdditionalData() { Data = "$$", Description = $"スパチャ : {item.snippet.superChatDetails.amountDisplayString}", Enable = true });
                    commentData.Comment = item.snippet.superChatDetails.userComment;
                }

                commentData.Additional = list.ToArray();
                OnCommentReceived?.Invoke(this, new(commentData));
            }
        }

        public void ShowWindow(Window window)
        {
            window.Title = "Twitch設定";
            window.Closed += (_, _) => setting.Save();
            window.Content = new SettingPage();
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}

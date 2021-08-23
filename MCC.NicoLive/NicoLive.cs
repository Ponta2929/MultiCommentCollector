using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MCC.NicoLive
{
    public class NicoLive : IPluginSender, ILogged
    {
        private Dictionary<string, string> header = new Dictionary<string, string>();

        private string liveId;
        private bool resume;
        private WebSocketClient viewingClient = new();
        private WebSocketClient chatClient = new();

        private const string message_1 = "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"abr\",\"protocol\":\"hls\",\"latency\":\"low\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";
        private const string message_2 = "{\"type\":\"getAkashic\",\"data\":{\"chasePlay\":false}}";
        private const string message_pong = "{\"type\":\"pong\"}";
        private const string message_keepSeat = "{\"type\":\"keepSeat\"}";
        private string message_chat = "";

        public string Author => "ぽんた";

        public string PluginName => "ニコニコ生放送";

        public string Description => "ニコニコ生放送の配信中のコメントを取得します。";

        public string Version => "1.0.0";

        public string SiteName => "NicoLive";

        public event CommentReceivedEventHandler OnCommentReceived;
        public event LoggedEventHandler OnLogged;

        private JsonSerializerOptions options = new();

        public bool Activate()
        {
            resume = true;

            if (!viewingClient.Connected)
            {
                Task.Run(Connect);
            }

            return true;
        }

        public bool Inactivate()
        {
            resume = false;

            viewingClient.Abort();
            chatClient.Abort();

            return true;
        }

        public bool IsSupport(string url)
        {
            var livePage = url.RegexString(@"https://live[\d]*.nicovideo.jp/watch/(?<value>[\w]+)", "value");
            var communityPage = url.RegexString(@"https://com.nicovideo.jp/community/(?<value>[\w]+)", "value");

            liveId = !livePage.Equals("") ? livePage : communityPage;

            if (!livePage.Equals("") || !communityPage.Equals(""))
                return true;

            return false;
        }

        public void PluginClose()
        {
            viewingClient.Abort();
            chatClient.Abort();

            chatClient.OnLogged -= Client_OnLogged;
        }

        public void PluginLoad()
        {
            options.Converters.Add(new DateTimeOffsetConverter());
            header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100");

            chatClient.OnLogged += Client_OnLogged;
        }

        private async void Connect()
        {
            while (resume)
            {
                if (!viewingClient.Connected)
                {
                    var get = Http.Get($"https://live2.nicovideo.jp/watch/{liveId}");
                    var index = get.IndexOf("data-props=\"");
                    var last = get.IndexOf("\">", index);
                    var result = get.Substring(index, last - index).Replace("data-props=\"", "");
                    var decode = HttpUtility.HtmlDecode(result);
                    var json = JsonSerializer.Deserialize<NicoLiveJson>(decode);

                    if (json.Site.ReLive.WebSocketURL is not null && !json.Site.ReLive.WebSocketURL.Equals(""))
                    {
                        viewingClient.URL = new(json.Site.ReLive.WebSocketURL);
                        viewingClient.Start(ViewingProcess, header);

                        await Task.Run(() =>
                        {
                            while (true)
                            {
                                if (chatClient.URL is not null)
                                    break;
                                Task.Delay(1000);
                            }

                        }).ContinueWith(t =>
                        {
                            chatClient.Start(ChatProcess, header);
                        });
                    }
                }

                await Task.Delay(60000);
            }
        }

        protected async void ViewingProcess(ClientWebSocket socket)
        {
            // 初回データ送信
            viewingClient.Send(message_1);
            viewingClient.Send(message_2);

            var received = new List<byte>();
            var buffer = new byte[4096];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    received.Clear();

                    var segment = new ArraySegment<byte>(buffer);
                    var count = 0;

                    while (true)
                    {
                        var result = await socket.ReceiveAsync(segment, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                            return;

                        received.AddRange(buffer);
                        count += result.Count;

                        if (result.EndOfMessage)
                            break;
                    }

                    var receive = Encoding.UTF8.GetString(received.ToArray(), 0, count);
                    var type = JsonSerializer.Deserialize<Receive>(receive);

                    if (type.Type == "room")
                    {
                        message_chat = "[{\"ping\": {\"content\": \"rs:0\"}},{\"ping\": {\"content\": \"ps:0\"}},{\"thread\": {\"thread\": \"" + type.Data.ThreadId + "\",\"version\": \"20061206\",\"user_id\": \"guest\",\"res_from\": 0,\"with_global\": 1,\"scores\": 1,\"nicoru\": 0}},{\"ping\": {\"content\": \"pf:0\"}},{\"ping\": {\"content\": \"rf:0\"}}]";
                        chatClient.URL = new(type.Data.MessageServer.URI);
                    }
                    else if (type.Type == "ping")
                    {
                        viewingClient.Send(message_pong);
                        viewingClient.Send(message_keepSeat);
                    }
                }
            }
            catch (WebSocketException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] 接続エラーが発生しました。");
            }
            catch (JsonException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] デコードエラーが発生しました。");
            }
            catch (OperationCanceledException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] 受信データ待受エラーが発生しました。");
            }
            catch (Exception e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
            }
            finally
            {
                // 終了
                viewingClient.Abort();
            }
        }

        protected async void ChatProcess(ClientWebSocket socket)
        {
            // 初回データ送信
            chatClient.Send(message_chat);

            var received = new List<byte>();
            var buffer = new byte[4096];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    received.Clear();

                    var segment = new ArraySegment<byte>(buffer);
                    var count = 0;

                    while (true)
                    {
                        var result = await socket.ReceiveAsync(segment, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                            return;

                        received.AddRange(buffer);
                        count += result.Count;

                        if (result.EndOfMessage)
                            break;
                    }

                    var receive = Encoding.UTF8.GetString(received.ToArray(), 0, count);

                    if (receive.Contains("chat"))
                    {
                        var data = JsonSerializer.Deserialize<ReceiveChat>(receive, options);

                        var comment = new CommentData()
                        {
                            LiveName = "NicoLive",
                            PostType = PostType.Comment,
                            Comment = ConvertHTMLCode(data.Chat.Content),
                            UserID = data.Chat.UserId,
                            PostTime = data.Chat.Date.LocalDateTime,
                            UserName = ""
                        };

                        OnCommentReceived?.Invoke(this, new(comment));
                    }
                }
            }
            catch (WebSocketException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] 接続エラーが発生しました。");
            }
            catch (JsonException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] デコードエラーが発生しました。");
            }
            catch (OperationCanceledException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] 受信データ待受エラーが発生しました。");
            }
            catch (Exception e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
            }
            finally
            {
                // 終了
                chatClient.Abort();
            }
        }

        private void Client_OnLogged(object sender, LoggedEventArgs e)
        {
            OnLogged?.Invoke(this, e);
        }

        public void Logged(LogLevel level, string message)
        {
            OnLogged?.Invoke(this, new(level, message));
        }

        public string ConvertHTMLCode(string message)
        {
            var reg = @"<a\s+[^>]*href\s*=\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>";
            var r = new Regex(reg, RegexOptions.IgnoreCase);
            var collection = r.Matches(message);

            foreach (Match m in collection)
            {
                if (m.Success)
                    return m.Groups["text"].Value.Trim();
            }

            return message;
        }
    }
}

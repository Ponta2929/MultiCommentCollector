﻿using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MCC.NicoLive
{
    public class NicoLive : IPluginSender, ILogged
    {
        private string liveId;
        private WebSocketClient viewingClient = new WebSocketClient();
        private WebSocketClient chatClient = new WebSocketClient();

        private string message_1 = "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"abr\",\"protocol\":\"hls\",\"latency\":\"low\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";// "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"abr\",\"protocol\":\"hls\",\"latency\":\"low\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":True},\"reconnect\":false}}";
        private string message_2 = "{\"type\":\"getAkashic\",\"data\":{\"chasePlay\":false}}";//"{\"type\":\"getAkashic\",\"data\":{\"chasePlay\":false}}";
        private string message_chat = "";
        private string message_pong = "{\"type\":\"pong\"}";
        private string message_keepSeat = "{\"type\":\"keepSeat\"}";

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
            if (!viewingClient.Connected)
            {
                Task.Run(() => Connect());
            }

            return true;
        }

        private void Connect()
        {
            var get = Http.Get($"https://live2.nicovideo.jp/watch/{liveId}");
            var index = get.IndexOf("data-props=\"");
            var last = get.IndexOf("\">", index);
            var result = get.Substring(index, last - index).Replace("data-props=\"", "");
            var decode = HttpUtility.HtmlDecode(result);
            var json = JsonSerializer.Deserialize<NicoLiveJson>(decode);

            viewingClient.OnLogged += OnLogged;
            chatClient.OnLogged += OnLogged;
            viewingClient.URL = new Uri(json.Site.ReLive.WebSocketURL);
            viewingClient.Start(ViewingProcess);

            Task.Run(() =>
            {
                while (true)
                {
                    if (chatClient.URL is not null)
                        break;
                    Task.Delay(1000);
                }

            }).ContinueWith(t =>
            {
                chatClient.Start(ChatProcess);
            });
        }

        public bool Inactivate()
        {
            viewingClient.Abort();
            chatClient.Abort();

            return true;
        }

        public bool IsSupport(string url)
        {
            liveId = url.RegexString(@"https://live2.nicovideo.jp/watch/(?<value>[\w]+)", "value");

            if (!liveId.Equals(""))
                return true;

            return false;
        }

        public void PluginClose()
        {
            viewingClient.Abort();
            chatClient.Abort();
        }

        public void PluginLoad()
        {
            options.Converters.Add(new DateTimeOffsetConverter());
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
            catch (WebSocketException)
            {
                Logged($"接続エラーが発生しました。");
            }
            catch (JsonException)
            {
                Logged($"デコードエラーが発生しました。");
            }
            catch (OperationCanceledException)
            {
                Logged($"受信データ待受エラーが発生しました。");
            }
            catch (Exception e)
            {
                Logged($"未知のエラーが発生しました。 : {e.Message.ToString()}");
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
                            Comment = data.Chat.Content,
                            UserID = data.Chat.UserId,
                            PostTime = data.Chat.Date.LocalDateTime,
                            UserName = ""
                        };

                        OnCommentReceived?.Invoke(this, new(comment));
                    }
                }
            }
            catch (WebSocketException)
            {
                Logged($"接続エラーが発生しました。");
            }
            catch (JsonException)
            {
                Logged($"デコードエラーが発生しました。");
            }
            catch (OperationCanceledException)
            {
                Logged($"受信データ待受エラーが発生しました。");
            }
            catch (Exception e)
            {
                Logged($"未知のエラーが発生しました。 : {e.Message.ToString()}");
            }
            finally
            {
                // 終了
                chatClient.Abort();
            }
        }
        public void Logged(string message)
        {
            OnLogged?.Invoke(this, new(message));
        }
    }
}

using MCC.NicoLive.Converter;
using MCC.Utility;
using MCC.Utility.Net;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MCC.NicoLive
{
    internal class NicoLiveConnector : ILogged
    {
        private Dictionary<string, string> header = new();
        private JsonSerializerOptions options = new();
        private readonly string message_1 = "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"abr\",\"protocol\":\"hls\",\"latency\":\"low\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";
        private readonly string message_2 = "{\"type\":\"getAkashic\",\"data\":{\"chasePlay\":false}}";
        private readonly string message_pong = "{\"type\":\"pong\"}";
        private readonly string message_keepSeat = "{\"type\":\"keepSeat\"}";
        private string message_chat = "";

        protected WebSocketClient ViewingClient { get; private set; } = new();
        protected WebSocketClient ChatClient { get; private set; } = new();

        public event LoggedEventHandler OnLogged;

        public event ChatReceivedEventHandler OnReceived;

        public bool Resume { get; set; }

        public NicoLiveConnector()
        {
            // ViewingClient.OnLogged += BaseLogged;
            ChatClient.OnLogged += BaseLogged;

            options.Converters.Add(new DateTimeOffsetConverter());
            options.Converters.Add(new BooleanConverter());
            header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100");
        }

        public async void Connect(string streamKey)
        {
            while (Resume)
            {
                if (!ViewingClient.Connected)
                {
                    var get = Http.Get($"https://live2.nicovideo.jp/watch/{streamKey}");
                    var index = get.IndexOf("data-props=\"");
                    var last = get.IndexOf("\">", index);
                    var result = get.Substring(index, last - index).Replace("data-props=\"", "");
                    var decode = HttpUtility.HtmlDecode(result);
                    var json = JsonSerializer.Deserialize<NicoLiveJson>(decode);

                    if (!string.IsNullOrEmpty(json.Site.ReLive.WebSocketURL))
                    {
                        ViewingClient.URL = new(json.Site.ReLive.WebSocketURL);
                        ViewingClient.Start(ViewingProcess, header);

                        await Task.Run(() =>
                        {
                            while (true)
                            {
                                if (ChatClient.URL is not null)
                                {
                                    break;
                                }

                                Task.Delay(1000);
                            }

                        }).ContinueWith(t =>
                        {
                            ChatClient.Start(ChatProcess, header);
                        });
                    }
                }

                await Task.Delay(60000);
            }
        }
        private async void ViewingProcess(ClientWebSocket socket)
        {
            // 初回データ送信
            ViewingClient.Send(message_1);
            ViewingClient.Send(message_2);

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
                        {
                            return;
                        }

                        received.AddRange(buffer);
                        count += result.Count;

                        if (result.EndOfMessage)
                        {
                            break;
                        }
                    }

                    var receive = Encoding.UTF8.GetString(received.ToArray(), 0, count);
                    var type = JsonSerializer.Deserialize<Receive>(receive);

                    if (type.Type == "room")
                    {
                        message_chat = "[{\"ping\": {\"content\": \"rs:0\"}},{\"ping\": {\"content\": \"ps:0\"}},{\"thread\": {\"thread\": \"" + type.Data.ThreadId + "\",\"version\": \"20061206\",\"user_id\": \"guest\",\"res_from\": 0,\"with_global\": 1,\"scores\": 1,\"nicoru\": 0}},{\"ping\": {\"content\": \"pf:0\"}},{\"ping\": {\"content\": \"rf:0\"}}]";
                        ChatClient.URL = new(type.Data.MessageServer.URI);
                    }
                    else if (type.Type == "ping")
                    {
                        ViewingClient.Send(message_pong);
                        ViewingClient.Send(message_keepSeat);

                        Logged(LogLevel.Debug, $"pong {message_pong}");
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
                Abort();
            }
        }

        private async void ChatProcess(ClientWebSocket socket)
        {
            // 初回データ送信
            ChatClient.Send(message_chat);

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
                        {
                            return;
                        }

                        received.AddRange(buffer);
                        count += result.Count;

                        if (result.EndOfMessage)
                        {
                            break;
                        }
                    }

                    var receive = Encoding.UTF8.GetString(received.ToArray(), 0, count);

                    if (receive.Contains("chat"))
                    {
                        var data = JsonSerializer.Deserialize<JsonData>(receive, options);

                        OnReceived?.Invoke(this, new(data));

                        if (data.Chat.Premium == 3 && data.Chat.Content.Contains("/disconnect"))
                        {
                            break;
                        }
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
                Abort();
            }
        }

        public void Abort()
        {
            ViewingClient.Abort();
            ChatClient.Abort();
        }

        private void Logged(LogLevel level, string message) => BaseLogged(this, new(level, message));

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}

using MCC.Utility;
using MCC.Utility.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.TwitCasting
{
    class TwitCastingConnector : ILogged
    {
        private bool called;
        private LatestMovie latest;
        private JsonSerializerOptions options = new();

        public WebSocketClient WebSocketClient = new();

        public bool Resume { get; set; }

        public event LoggedEventHandler OnLogged;

        public event ChatReceivedEventHandler OnReceived;


        public TwitCastingConnector()
        {
            WebSocketClient.OnLogged += BaseLogged;

            options.Converters.Add(new DateTimeOffsetConverter());
        }

        public async void Connect(string streamKey)
        {
            // 初回ライブ情報を取得
            latest = GetLatestMovie(streamKey);

            while (Resume)
            {
                if (latest?.Movie.IsOnLive == true)
                {
                    var chatUrl = GetChatWebSocket(latest.Movie.ID);

                    if (!chatUrl.Equals(string.Empty))
                    {
                        called = false;

                        WebSocketClient.URL = new(chatUrl);

                        // 開始
                        WebSocketClient.Start(Process);
                    }
                }
                else if (latest?.Movie.IsOnLive == false && !called)
                {
                    called = true;

                    Logged(LogLevel.Info, "現在配信を行っていません。");
                }

                await Task.Delay(5000);
            }
        }

        private async void Process(ClientWebSocket client)
        {
            var received = new List<byte>();
            var buffer = new byte[4096];

            try
            {
                while (client.State == WebSocketState.Open)
                {
                    received.Clear();

                    var segment = new ArraySegment<byte>(buffer);
                    var count = 0;

                    while (true)
                    {
                        var result = await client.ReceiveAsync(segment, CancellationToken.None);

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

                    var message = Encoding.UTF8.GetString(received.ToArray(), 0, count);
                    var receive = JsonSerializer.Deserialize<JsonData[]>(message, options);

                    foreach (var comment in receive)
                    {
                        if (comment.Type.Equals("comment"))
                        {
                            OnReceived?.Invoke(this, new(comment));
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
                // 終了
                WebSocketClient.Abort();
            }
        }

        /// <summary>
        /// 一分ごとに動画IDをチェックする
        /// </summary>
        public async void CheckMovieId(string streamKey)
        {
            while (Resume)
            {
                // 動画IDを60秒ごとにチェックする
                await Task.Delay(60000);

                var movie = GetLatestMovie(streamKey);

                if (latest?.Movie.ID != movie?.Movie.ID)
                {
                    WebSocketClient.Abort();

                    latest = movie;

                    Logged(LogLevel.Info, "ライブIDの変更を検知しました。");
                }
            }
        }

        private string GetChatWebSocket(int movieId)
        {
            var postData = new Hashtable();
            postData["movie_id"] = movieId;

            var result = Http.Post($"https://twitcasting.tv/eventpubsuburl.php", postData);

            if (string.IsNullOrEmpty(result))
            {
                Logged(LogLevel.Info, "チャット接続先情報を取得できませんでした。");

                return string.Empty;
            }

            return JsonSerializer.Deserialize<EventPubSubURL>(result).URL;
        }

        private LatestMovie GetLatestMovie(string userId)
        {
            var result = Http.Get($"https://frontendapi.twitcasting.tv/users/{userId}/latest-movie");

            if (string.IsNullOrEmpty(result))
            {
                Logged(LogLevel.Info, "配信情報を取得できませんでした。");

                return null;
            }

            return JsonSerializer.Deserialize<LatestMovie>(result);
        }

        public void Abort() => WebSocketClient.Abort();

        private void Logged(LogLevel level, string message) => BaseLogged(this, new(level, message));

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}

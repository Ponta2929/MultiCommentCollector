using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.TwitCasting
{
    public class TwitCasting : IPluginSender, ILogged
    {
        private ClientWebSocket client;

        private string userId;
        private int movieId;
        private bool connect;

        public string Author => "ぽんた";

        public string PluginName => "TwitCasting";

        public string Description => "TwitCastingの配信中のコメントを取得します。";

        public string Version => "1.0.0";

        public string SiteName => "TwitCasting";

        public event LoggedEventHandler OnLogged;
        public event CommentReceivedEventHandler OnCommentReceived;

        private string ServerName { get; set; } = "localhost";
        private int Port { get; set; } = 29291;

        private JsonSerializerOptions options = new();

        public TwitCasting()
        {
        }

        public bool Activate()
        {
            if (!connect)
            {
                connect = true;

                Task.Run(() => Connect());
                Task.Run(() => CheckMovieId());
            }
            return connect;
        }

        public bool Inactivate()
        {
            connect = false;

            Abort();

            return !connect;
        }

        public void PluginClose()
        {
            Abort();
        }

        public void PluginLoad()
        {
            options.Converters.Add(new DateTimeOffsetConverter());
        }

        private async void Connect()
        {
            if (client is null)
                client = new();

            while (connect)
            {
                // ツイキャス側クライアント
                if (client.State == WebSocketState.None || client.State == WebSocketState.Closed || client.State == WebSocketState.Aborted)
                {
                    if (client.State == WebSocketState.Closed || client.State == WebSocketState.Aborted)
                    {
                        OnLogged?.Invoke(this, new($"TwitCastingへ再接続をします。"));
                        client.Dispose();
                        client = new();
                    }

                    try
                    {
                        // 動画情報を取得
                        var latest = GetLatestMovie(userId);

                        if (latest.Movie.IsOnLive)
                        {
                            await client.ConnectAsync(new(GetChatWebSocket(movieId = latest.Movie.ID)), CancellationToken.None);

                            OnLogged?.Invoke(this, new($"TwitCastingへ接続を開始しました。"));

                            // 受信開始
                            Receive();
                        }
                    }
                    catch
                    {
                        OnLogged?.Invoke(this, new($"接続時にエラーが発生しました。"));
                    }
                }

                await Task.Delay(5000);
            }
        }

        /// <summary>
        /// 一分ごとに動画IDをチェックする
        /// </summary>
        private async void CheckMovieId()
        {
            while (connect)
            {
                // 動画IDを60秒ごとにチェックする
                await Task.Delay(60000);

                var latest = GetLatestMovie(userId);

                if (movieId != latest.Movie.ID)
                {
                    Abort();
                }
            }
        }

        private async void Receive()
        {
            await Task.Run(async () =>
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
                                break;
                        }

                        var message = Encoding.UTF8.GetString(received.ToArray(), 0, count);
                        var receive = JsonSerializer.Deserialize<Comment[]>(message, options);

                        foreach (var comment in receive)
                        {
                            if (comment.Type.Equals("comment"))
                            {
                                var commentData = new CommentData()
                                {
                                    LiveName = "TwitCasting",
                                    PostTime = comment.CreatedAt.LocalDateTime,
                                    Comment = comment.Message,
                                    UserName = comment.Author.Name,
                                    UserID = comment.Author.ID
                                };

                                OnCommentReceived?.Invoke(this, new(commentData));
                            }
                        }
                    }
                }
                catch (WebSocketException)
                {
                    OnLogged?.Invoke(this, new($"接続エラーが発生しました。"));
                }
                catch (JsonException)
                {
                    OnLogged?.Invoke(this, new($"デコードエラーが発生しました。"));
                }
                catch (Exception e)
                {
                    OnLogged?.Invoke(this, new($"未知のエラーが発生しました。 : {e.Message.ToString()}"));
                }
                finally
                {

                    // 終了
                    Abort();
                }
            });
        }

        private void Abort()
        {
            try
            {
                if (client is not null && client.State == WebSocketState.Open)
                {
                    client.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                    client.Abort();
                }
            }
            catch
            {
            }
            finally
            {
                if (client is not null)
                    client.Dispose();

                OnLogged?.Invoke(this, new($"TwitCastingへの接続が終了しました。"));
            }
        }

        private string GetChatWebSocket(int movieId)
        {
            var postData = new Hashtable();
            postData["movie_id"] = movieId;

            return JsonSerializer.Deserialize<EventPubSubURL>(Http.Post($"https://twitcasting.tv/eventpubsuburl.php", postData)).URL;
        }

        private LatestMovie GetLatestMovie(string userId)
            => JsonSerializer.Deserialize<LatestMovie>(Http.Get($"https://frontendapi.twitcasting.tv/users/{userId}/latest-movie"));

        public bool IsSupport(string url)
        {
            userId = url.RegexString(@"https://twitcasting.tv/(?<value>[\w]+)", "value");

            if (!userId.Equals(""))
                return true;

            return false;
        }
    }
}

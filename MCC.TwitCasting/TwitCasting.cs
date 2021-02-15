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
        private ClientWebSocket twitcas_client;

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
            if (twitcas_client is null)
                twitcas_client = new();

            while (connect)
            {
                // ツイキャス側クライアント
                if (twitcas_client.State == WebSocketState.None || twitcas_client.State == WebSocketState.Closed || twitcas_client.State == WebSocketState.Aborted)
                {
                    if (twitcas_client.State == WebSocketState.Closed || twitcas_client.State == WebSocketState.Aborted)
                    {
                        OnLogged?.Invoke(this, new($"TwitCastingへ再接続をします。"));
                        twitcas_client.Dispose();
                        twitcas_client = new();
                    }

                    try
                    {
                        // 動画情報を取得
                        var latest = GetLatestMovie(userId);

                        if (latest.Movie.IsOnLive)
                        {
                            await twitcas_client.ConnectAsync(new(GetChatWebSocket(movieId = latest.Movie.ID)), CancellationToken.None);

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
                    while (twitcas_client.State == WebSocketState.Open)
                    {
                        var segment = new ArraySegment<byte>(buffer);
                        var result = await twitcas_client.ReceiveAsync(segment, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Abort();

                            return;
                        }

                        int count = result.Count;
                        received.Clear();
                        received.AddRange(buffer);

                        while (!result.EndOfMessage)
                        {
                            result = await twitcas_client.ReceiveAsync(segment, CancellationToken.None);
                            received.AddRange(buffer);
                            count += result.Count;
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
                catch (Exception e)
                {
                    OnLogged?.Invoke(this, new(e.Message));
                }
                finally
                {
                    OnLogged?.Invoke(this, new($"TwitCastingへの接続が終了しました。"));

                    // 終了
                    Abort();
                }
            });
        }

        private void Abort()
        {
            try
            {
                if (twitcas_client is not null && twitcas_client.State == WebSocketState.Open)
                {
                    twitcas_client.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                    twitcas_client.Abort();
                }
            }
            catch
            {
            }
            finally
            {
                if (twitcas_client is not null)
                    twitcas_client.Dispose();
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

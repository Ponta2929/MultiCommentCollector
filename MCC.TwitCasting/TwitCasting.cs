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
    public class TwitCasting : WebSocketClient, IPluginSender
    {
        private string userId;
        private LatestMovie latest;
        private bool resume;

        public string Author => "ぽんた";

        public string PluginName => "TwitCasting";

        public string Description => "TwitCastingの配信中のコメントを取得します。";

        public string Version => "1.0.0";

        public string SiteName => "TwitCasting";

        public event CommentReceivedEventHandler OnCommentReceived;

        private JsonSerializerOptions options = new();

        public TwitCasting()
        {

        }

        public bool Activate()
        {
            resume = true;

            if (!Connected)
            {
                Task.Run(() => Connect());
                Task.Run(() => CheckMovieId());
            }
            return true;
        }

        public bool Inactivate()
        {
            resume = false;

            Abort();

            return true;
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
            // 初回ライブ情報を取得
            latest = GetLatestMovie(userId);

            while (resume)
            {
                if (latest.Movie.IsOnLive)
                {
                    URL = new(GetChatWebSocket(latest.Movie.ID));

                    // 開始
                    Start();
                }

                await Task.Delay(5000);
            }
        }

        protected override async void Process(ClientWebSocket client)
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
                Abort();
            }
        }

        /// <summary>
        /// 一分ごとに動画IDをチェックする
        /// </summary>
        private async void CheckMovieId()
        {
            while (resume)
            {
                // 動画IDを60秒ごとにチェックする
                await Task.Delay(60000);

                var movie = GetLatestMovie(userId);

                if (latest.Movie.ID != movie.Movie.ID)
                {
                    Abort();

                    latest = movie;

                    Logged("ライブIDの変更を検知しました。");
                }
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

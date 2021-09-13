using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
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
    public class TwitCasting : WebSocketClient, IPluginSender
    {
        private LatestMovie latest;
        private bool resume;
        private bool called;

        public string Author => "ぽんた";

        public string PluginName => "TwitCasting";

        public string StreamKey { get; set; }

        public string Description => "TwitCastingの配信中のコメントを取得します。";

        public string Version => "1.0.0";

        public string SiteName => "TwitCasting";

        public event CommentReceivedEventHandler OnCommentReceived;

        private JsonSerializerOptions options = new();

        public bool Activate()
        {
            resume = true;

            if (!Connected)
            {
                Task.Run(Connect);
                Task.Run(CheckMovieId);
            }

            return true;
        }

        public bool Inactivate()
        {
            resume = false;

            Abort();

            return true;
        }

        public void PluginClose() => Abort();

        public void PluginLoad() => options.Converters.Add(new DateTimeOffsetConverter());

        private async void Connect()
        {
            // 初回ライブ情報を取得
            latest = GetLatestMovie(StreamKey);

            while (resume)
            {
                if (latest?.Movie.IsOnLive == true)
                {
                    var chatUrl = GetChatWebSocket(latest.Movie.ID);

                    if (!chatUrl.Equals(string.Empty))
                    {
                        called = false;

                        URL = new(chatUrl);

                        // 開始
                        Start();
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
                        {
                            break;
                        }
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

                var movie = GetLatestMovie(StreamKey);

                if (latest?.Movie.ID != movie?.Movie.ID)
                {
                    Abort();

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

            if (result.Equals(""))
            {
                Logged(LogLevel.Info, "チャット接続先情報を取得できませんでした。");

                return string.Empty;
            }

            return JsonSerializer.Deserialize<EventPubSubURL>(result).URL;
        }

        private LatestMovie GetLatestMovie(string userId)
        {
            var result = Http.Get($"https://frontendapi.twitcasting.tv/users/{userId}/latest-movie");

            if (result.Equals(string.Empty))
            {
                Logged(LogLevel.Info, "配信情報を取得できませんでした。");

                return null;
            }

            return JsonSerializer.Deserialize<LatestMovie>(result);
        }
        public bool IsSupport(string url)
        {
            StreamKey = url.RegexString(@"https://twitcasting.tv/(?<value>[\w]+)", "value");

            if (!StreamKey.Equals(""))
            {
                return true;
            }

            return false;
        }
    }
}

using MCC.Utility;
using MCC.Utility.Net;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MCC.Core.Server
{
    /// <summary>
    /// コメント受信サーバー
    /// </summary>
    public class CommentReceiverServer : WebSocketServer
    {
        #region Singleton

        private static CommentReceiverServer instance;
        public static CommentReceiverServer GetInstance() => instance ?? (instance = new());
        public static void SetInstance(CommentReceiverServer inst) => instance = inst;

        #endregion

        public CommentReceiverServer() : this("localhost", 29291) { }

        public CommentReceiverServer(string serverName, int port) : base(serverName, port) { }

        public event CommentReceivedEventHandler OnCommentReceived;

        /// <summary>
        /// コメント受信
        /// </summary>
        protected override async void Process(WebSocket socket)
        {
            try
            {
                var received = new List<byte>();
                var buffer = new byte[4096];

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
                    var dataType = JsonSerializer.Deserialize<PostHeader>(receive);

                    // コメント処理
                    if (dataType.PostType == PostType.Comment)
                        OnCommentReceived?.Invoke(this, new(JsonSerializer.Deserialize<CommentData>(receive)));
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
            catch (Exception e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
            }
            finally
            {
                Close(socket);
            }
        }
    }
}

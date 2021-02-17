using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MCC.Core
{
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

        protected override async void Process(WebSocket socket)
        {
            try
            {
                var received = new List<byte>();
                var buffer = new byte[4096];

                while (socket.State == WebSocketState.Open)
                {
                    var segment = new ArraySegment<byte>(buffer);
                    var result = await socket.ReceiveAsync(segment, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    int count = result.Count;
                    received.Clear();
                    received.AddRange(buffer);

                    while (!result.EndOfMessage)
                    {
                        result = await socket.ReceiveAsync(segment, CancellationToken.None);
                        received.AddRange(buffer);
                        count += result.Count;
                    }

                    var receive = Encoding.UTF8.GetString(received.ToArray(), 0, count);
                    var dataType = JsonSerializer.Deserialize<PostHeader>(receive);

                    // コメント処理
                    if (dataType.PostType == PostType.Comment)
                        OnCommentReceived?.Invoke(this, new(JsonSerializer.Deserialize<CommentData>(receive)));
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
            catch (Exception e)
            {
                Logged($"未知のエラーが発生しました。 : {e.Message.ToString()}");
            }
            finally
            {
                Close(socket);
            }
        }
    }
}

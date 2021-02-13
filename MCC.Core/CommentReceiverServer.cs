using MCC.Utility;
using System;
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
                var buffer = new byte[4096];

                while (socket.State == WebSocketState.Open)
                {
                    var segment = new ArraySegment<byte>(buffer);
                    var result = await socket.ReceiveAsync(segment, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                        return;
                    }

                    var receive = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var dataType = JsonSerializer.Deserialize<PostHeader>(receive);

                    // コメント処理
                    if (dataType.PostType == PostType.Comment)
                        OnCommentReceived?.Invoke(this, new(JsonSerializer.Deserialize<CommentData>(receive)));

                }
            }
            catch (Exception e)
            {
                Logged($"エラーが発生しました。{e.Message}");

                return;
            }
            finally
            {
                Logged($"接続が終了しました。");
            }
        }
    }
}

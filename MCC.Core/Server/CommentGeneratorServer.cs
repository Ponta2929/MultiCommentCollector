using MCC.Utility;
using MCC.Utility.IO;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace MCC.Core.Server
{
    /// <summary>
    /// コメント送信サーバー
    /// </summary>
    public sealed class CommentGeneratorServer : WebSocketServer
    {
        private object syncObject = new object();

        /// <summary>
        /// クラスの構造タイプです。
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// Xml形式。
            /// </summary>
            Xml,

            /// <summary>
            /// Json形式。
            /// </summary>
            Json
        }

        #region Singleton

        private static CommentGeneratorServer instance;
        public static CommentGeneratorServer Instance => instance ??= new();
        #endregion

        public CommentGeneratorServer() : this("localhost", 29292) { }

        public CommentGeneratorServer(string serverName, int port) : base(serverName, port) { }

        /// <summary>
        /// 終了メッセージ受信
        /// </summary>
        /// <param name="socket"></param>
        protected override async void Process(WebSocket socket)
        {
            try
            {
                var buffer = new byte[32];

                while (socket.State == WebSocketState.Open)
                {
                    var segment = new ArraySegment<byte>(buffer);
                    var result = await socket.ReceiveAsync(segment, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }
                }
            }
            catch (WebSocketException e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] 接続エラーが発生しました。");
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

        /// <summary>
        /// データ送信
        /// </summary>
        public void SendData<T>(T data, DataType type = DataType.Json) where T : class
        {
            lock (syncObject)
            {
                foreach (var socket in Sockets)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        var converted = XSSConvert(data);
                        var response = string.Empty;

                        if (type == DataType.Json)
                            response = System.Text.Json.JsonSerializer.Serialize<T>(converted as T);
                        else if (type == DataType.Xml)
                            response = XmlSerializer.Serialize<T>(converted);

                        var buffer = Encoding.UTF8.GetBytes(response);
                        var segment = new ArraySegment<byte>(buffer);

                        socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }

        /// <summary>
        /// クロスサイトスクリプト変換
        /// </summary>
        private object XSSConvert(object data)
        {
            foreach (var info in data.GetType().GetFields())
                if (info.FieldType == typeof(string))
                    info.SetValue(data, XSSFilter(info.GetValue(data) as string));

            foreach (var info in data.GetType().GetProperties())
                if (info.PropertyType == typeof(string))
                    info.SetValue(data, XSSFilter(info.GetValue(data) as string));

            return data;
        }

        private string XSSFilter(string data)
        {
            if (data is null)
                return null;

            data = data.Replace("<", "&lt;");
            data = data.Replace(">", "&gt;");
            return data.Replace("\"", "&quot;");
        }
    }
}

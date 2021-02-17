using MCC.Utility;
using MCC.Utility.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.Core
{
    public sealed class CommentGeneratorServer : WebSocketServer
    {
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
        public static CommentGeneratorServer GetInstance() => instance ?? (instance = new());
        public static void SetInstance(CommentGeneratorServer inst) => instance = inst;

        #endregion

        public CommentGeneratorServer() : this("localhost", 29292) { }

        public CommentGeneratorServer(string serverName, int port) : base(serverName, port) { }

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
            catch (WebSocketException)
            {
                Logged($"接続エラーが発生しました。");
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

        public async void SendData<T>(T data, DataType type = DataType.Json)
        {
            foreach (var socket in sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var converted = XSSConvert(data);
                    var response = string.Empty;

                    if (type == DataType.Json)
                        response = System.Text.Json.JsonSerializer.Serialize<T>((T)converted);
                    else if (type == DataType.Xml)
                        response = XmlSerializer.Serialize<T>(converted);

                    var buffer = Encoding.UTF8.GetBytes(response);
                    var segment = new ArraySegment<byte>(buffer);

                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private object XSSConvert(object data)
        {
            foreach (var info in data.GetType().GetFields())
                if (info.FieldType == typeof(string))
                    info.SetValue(data, XSSFilter((string)info.GetValue(data)));

            foreach (var info in data.GetType().GetProperties())
                if (info.PropertyType == typeof(string))
                    info.SetValue(data, XSSFilter((string)info.GetValue(data)));

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

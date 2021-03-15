using MCC.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.Core.Server
{
    public class WebSocketServer : ILogged
    {
        private object syncObject = new();
        private HttpListener listener = new();
        protected List<WebSocket> Sockets = new();

        /// <summary>
        /// 接続先のサーバー名
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 接続先のポート番号
        /// </summary>
        public int Port { get; set; }

        public event LoggedEventHandler OnLogged;

        public WebSocketServer(string serverName, int port)
            => (ServerName, Port) = (serverName, port);

        public void Start()
        {
            Task.Run(() => StartListen());
        }

        public void Stop()
        {
            Abort();
        }

        private async void StartListen()
        {
            if (listener is null)
                listener = new();

            if (!listener.IsListening)
            {
                try
                {
                    // リスナー設定
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add($"http://{ServerName}:{Port}/");

                    Logged(LogLevel.Debug, $"[{ServerName}:{Port}]");
                    Logged(LogLevel.Info, $"サーバーを開始します。[{ServerName}:{Port}]");

                    listener.Start();

                    while (listener.IsListening)
                    {
                        Logged(LogLevel.Info, $"接続要求を待機しています。");

                        var context = await listener.GetContextAsync();

                        if (context.Request.IsWebSocketRequest)
                        {
                            Request(context);
                        }
                        else
                        {
                            using (var response = context.Response)
                            using (var stream = response.OutputStream)
                            using (var writer = new StreamWriter(stream, Encoding.UTF8))
                            {
                                response.StatusCode = 400;
                                writer.Write("Bad Request");
                                Logged(LogLevel.Warn, $"接続要求は破棄されました。");
                            }
                        }
                    }
                }
                catch (HttpListenerException e)
                {
                    Logged(LogLevel.Error, $"[{e.InnerException}] 指定のプレフィックスはすでに使用されています。");
                }
                catch (Exception e)
                {
                    Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
                }
                finally
                {
                    Abort();
                }
            }
        }

        private void Abort()
        {
            if (listener is not null)
            {
                // ソケットを閉じる
                Close();

                // リスナー削除
                listener.Abort();
                listener = null;
            }

            Logged(LogLevel.Info, $"サーバーを停止しました。");
        }

        private async void Request(HttpListenerContext context)
        {
            try
            {
                var task = await context.AcceptWebSocketAsync(null);

                Logged(LogLevel.Info, $"接続が開始されました。");

                lock (syncObject)
                {
                    Sockets.Add(task.WebSocket);
                }

                Process(task.WebSocket);
            }
            catch (Exception e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
            }
        }

        protected virtual void Process(WebSocket socket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 開いているソケットを終了させます。
        /// </summary>
        public void Close()
        {
            lock (syncObject)
            {
                for (var i = 0; i < Sockets.Count; i++)
                {
                    var socket = Sockets[i];

                    if (socket.State == WebSocketState.Open)
                        socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);

                    socket.Dispose();
                }

                Sockets.Clear();
            }

            Logged(LogLevel.Info, "すべての接続を閉じました。");
        }

        /// <summary>
        /// 開いている対象のソケットを終了させます。
        /// </summary>
        public void Close(WebSocket socket)
        {
            if (socket.State == WebSocketState.Open)
                socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);

            socket.Dispose();

            Logged(LogLevel.Info, "対象の接続を閉じました。");

            lock (syncObject)
            {
                Sockets.Remove(socket);
            }
        }

        /// <summary>
        /// ログ送信
        /// </summary>
        /// <param name="message"></param>
        public void Logged(LogLevel level, string message)
        {
            OnLogged?.Invoke(this, new(level, message));
        }
    }
}

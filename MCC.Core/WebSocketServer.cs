using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.Core
{
    public class WebSocketServer : ILogged
    {
        private object syncObject = new();
        private HttpListener listener = new();
        protected List<WebSocket> sockets = new();

        public string ServerName { get; set; }

        public int Port { get; set; }

        public bool IsClosed { get; private set; }

        public event LoggedEventHandler OnLogged;

        public WebSocketServer(string serverName, int port)
            => (ServerName, Port) = (serverName, port);

        public void Start()
        {
            Task.Run(() => StartListen());
        }

        public void Stop()
        {
            Logged($"サーバーを停止しました。");

            Abort();
        }

        private async void StartListen()
        {
            if (listener is null)
                listener = new();

            if (!listener.IsListening)
            {
                // リスナー設定
                listener.Prefixes.Clear();
                listener.Prefixes.Add($"http://{ServerName}:{Port}/");

                Logged($"サーバーを開始します。[{ServerName}:{Port}]");

                listener.Start();

                try
                {
                    while (listener.IsListening)
                    {
                        Logged($"接続要求を待機しています。");

                        var context = await listener.GetContextAsync();

                        if (context.Request.IsWebSocketRequest)
                        {
                            Logged($"ソケット接続が開始されました。");

                            Request(context);
                        }
                        else
                        {
                            using (var response = context.Response)
                            using (var stream = response.OutputStream)
                            {
                                Logged($"接続要求は破棄されました。");

                                var writeData = Encoding.UTF8.GetBytes("接続要求は破棄されました。");
                                response.StatusCode = 400;
                                stream.Write(writeData, 0, writeData.Length);
                                context.Response.Close();
                            }
                        }

                        await Task.Delay(1);
                    }
                }
                catch(Exception e)
                {

                }
                finally
                {
                    Logged($"サーバーが停止されました。");

                    Abort();
                }
            }
        }

        private void Abort()
        {
            if (listener is not null)
            {
                try
                {
                    Close();
                }
                catch
                {
                }
                finally
                {
                    listener?.Abort();
                    listener = null;
                }
            }
        }
        private async void Request(HttpListenerContext context)
        {
            var task = await context.AcceptWebSocketAsync(null);

            lock (syncObject)
            {
                sockets.Add(task.WebSocket);
            }

            Process(task.WebSocket);
        }

        protected virtual async void Process(WebSocket socket)
        {
            await Task.Delay(1);
        }

        /// <summary>
        /// 開いているソケットを終了させます。
        /// </summary>
        public async void Close()
        {
            foreach (var socket in sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                    socket.Dispose();
                }
            }

            Logged("すべてのソケットを閉じました。");

            lock (syncObject)
            {
                sockets.Clear();
            }
        }

        /// <summary>
        /// 開いている対象のソケットを終了させます。
        /// </summary>
        public async void Close(WebSocket socket)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                socket.Dispose();

                Logged("対象のソケットを閉じました。");
            }

            lock (syncObject)
            {
                sockets.Remove(socket);
            }
        }

        public void Logged(string message)
        {
            OnLogged?.Invoke(this, new(message));
        }
    }
}

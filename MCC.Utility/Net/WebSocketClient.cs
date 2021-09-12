using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.Utility.Net
{
    public class WebSocketClient : ILogged
    {
        protected ClientWebSocket client;

        /// <summary>
        /// 接続しているかどうか
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// 接続先URL
        /// </summary>
        public Uri URL { get; set; }

        public event LoggedEventHandler OnLogged;

        public async void Start(Dictionary<string, string> header = null)
        {
            Start(v => Process(v), header);
        }

        public async void Start(Action<ClientWebSocket> action, Dictionary<string, string> header = null)
        {
            if (client is null)
                client = new();

            if (!Connected)
            {
                try
                {
                    if (header is not null)
                    {
                        foreach (var item in header)
                        {
                            client.Options.SetRequestHeader(item.Key, item.Value);
                        }
                    }

                    await client.ConnectAsync(URL, CancellationToken.None);

                    Connected = true;

                    Logged(LogLevel.Info, $"接続を開始しました。");

                    // 受信開始
                    action(client);
                }
                catch (Exception e)
                {
                    Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
                }
            }
        }

        public async void Send(string message)
        {
            Send(message, Encoding.UTF8);
        }

        public async void Send(string message, Encoding encoding)
        {
            var buffer = encoding.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        protected virtual void Process(ClientWebSocket client)
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            if (client is not null)
            {
                if (client.State == WebSocketState.Open)
                {
                    client.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                }

                client.Abort();
                client.Dispose();
                client = null;

                URL = null;
                Connected = false;

                Logged(LogLevel.Info, $"接続を閉じました。");
            }
        }

        public void Logged(LogLevel level, string message)
        {
            OnLogged?.Invoke(this, new(level, message));
        }
    }
}

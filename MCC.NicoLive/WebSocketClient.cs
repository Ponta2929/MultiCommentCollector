using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.NicoLive
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

        public async void Start(Action<ClientWebSocket> action)
        {
            if (client is null)
                client = new();

            if (!Connected)
            {
                try
                {
                    client.Options.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100");

                    await client.ConnectAsync(URL, CancellationToken.None);

                    Connected = true;

                    Logged($"接続を開始しました。");

                    // 受信開始
                    action(client);
                }
                catch (Exception e)
                {
                    Logged($"未知のエラーが発生しました。 : {e.Message.ToString()}");
                }
            }
        }
        public async void Send(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
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

                Connected = false;

                Logged($"接続を閉じました。");
            }
        }

        public void Logged(string message)
        {
            OnLogged?.Invoke(this, new(message));
        }
    }
}

using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCC.TwitCasting
{
    public class WebSocketClient : ILogged
    {
        private ClientWebSocket client;

        /// <summary>
        /// 接続しているかどうか
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// 接続先URL
        /// </summary>
        public Uri URL { get; set; }

        public event LoggedEventHandler OnLogged;

        public async void Start()
        {
            if (client is null)
                client = new();

            if (!Connected)
            {
                try
                {
                    await client.ConnectAsync(URL, CancellationToken.None);

                    Connected = true;

                    Logged($"接続を開始しました。");

                    // 受信開始
                    Process(client);
                }
                catch (Exception e)
                {
                    Logged($"未知のエラーが発生しました。 : {e.Message.ToString()}");
                }
            }
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

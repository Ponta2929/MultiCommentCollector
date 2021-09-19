using MCC.Utility;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MCC.Twitch
{
    public class IrcClient : ILogged
    {
        protected TcpClient TcpClient;
        private StreamReader input;
        private StreamWriter output;

        public bool Connected { get; private set; }

        public event LoggedEventHandler OnLogged;

        public void Start(string ip, int port, string userName, string password, string channel) => Start(v => Process(v), ip, port, userName, password, channel);

        public async void Start(Action<StreamReader> action, string ip, int port, string userName, string password, string channel)
        {
            if (TcpClient is null)
            {
                TcpClient = new();
            }

            if (!Connected)
            {
                try
                {
                    await TcpClient.ConnectAsync(ip, port);
                    input = new(TcpClient.GetStream());
                    output = new(TcpClient.GetStream());

                    // Join
                    await output.WriteLineAsync("PASS " + password);
                    await output.WriteLineAsync("NICK " + userName);
                    await output.WriteLineAsync("USER " + userName + " 8 * :" + userName);
                    await output.WriteLineAsync("JOIN #" + channel);
                    await output.FlushAsync();

                    Connected = true;

                    Logged(LogLevel.Debug, $"[{ip}:{port}]");
                    Logged(LogLevel.Info, $"接続を開始しました。");

                    // Ping
                    await Task.Run(Ping);

                    // 受信開始
                    action(input);
                }
                catch (Exception e)
                {
                    Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
                }
            }
        }

        public async void Send(string message)
        {
            try
            {
                await output.WriteLineAsync(message);
                await output.FlushAsync();
            }
            catch (Exception e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
            }
        }

        private async void Ping()
        {
            while (Connected)
            {
                Send("PING irc.twitch.tv");

                Logged(LogLevel.Debug, $"PING irc.twitch.tv");

                // 5分待つ
                await Task.Delay(300000);
            }
        }

        protected virtual void Process(StreamReader reader) => throw new NotImplementedException();

        public void Abort()
        {
            if (TcpClient is not null)
            {
                output.Close();
                input.Close();
                TcpClient.Dispose();
                TcpClient = null;

                Connected = false;

                Logged(LogLevel.Info, $"接続を閉じました。");
            }
        }

        public void Logged(LogLevel level, string message) => OnLogged?.Invoke(this, new(level, message));
    }
}

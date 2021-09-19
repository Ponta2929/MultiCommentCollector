using MCC.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MCC.Twitch
{
    internal class TwitchConnector : ILogged
    {
        protected IrcClient IrcClient = new();

        public bool Resume { get; set; }

        public event LoggedEventHandler OnLogged;

        public event ChatReceivedEventHandler OnReceived;

        public TwitchConnector() => IrcClient.OnLogged += BaseLogged;

        public async void Connect(string password, string streamKey)
        {
            while (Resume)
            {
                IrcClient.Start(Process, "irc.twitch.tv", 6667, "mcc_twitch_bot", password, streamKey);

                await Task.Delay(10000);
            }
        }

        private async void Process(StreamReader reader)
        {
            try
            {
                while (IrcClient.Connected)
                {
                    var message = await reader.ReadLineAsync();

                    if (message != null)
                    {
                        if (message.Contains("PRIVMSG"))
                        {
                            OnReceived?.Invoke(this, new(message));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logged(LogLevel.Error, $"[{e.InnerException}] {e.Message.ToString()}");
            }
            finally
            {
                // 終了
                IrcClient.Abort();
            }
        }

        public void Abort() => IrcClient.Abort();

        private void Logged(LogLevel level, string message) => BaseLogged(this, new(level, message));

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}

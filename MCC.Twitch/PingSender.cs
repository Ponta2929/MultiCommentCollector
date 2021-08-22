using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Twitch
{
    public class PingSender
    {
        private IrcClient client;

        public PingSender(IrcClient irc)
        {
            client = irc;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (client.Connected)
                {
                    client.Send("PING irc.twitch.tv");

                    Logged(LogLevel.Debug, $"PING irc.twitch.tv");

                    // 5分待つ
                    await Task.Delay(300000);
                }
            });
        }

        private void Logged(LogLevel level, string message)
        {
            client.Logged(level, message);
        }
    }
}

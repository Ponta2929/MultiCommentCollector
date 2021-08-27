using MCC.Plugin;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MCC.Twitch
{
    public class Twitch : IrcClient, IPluginSender, ISetting
    {
        private Setting setting = Setting.Instance;

        public string SiteName => "Twitch";

        public string Author => "ぽんた";

        public string PluginName => "Twitch";

        public string Description => "Twitchのコメントを取得します。";

        public string Version => "1.0.0";

        public string MenuItemName => "設定";

        public event CommentReceivedEventHandler OnCommentReceived;

        private string userId;
        private bool resume;

        public bool Activate()
        {
            resume = true;

            if (!Connected)
            {
                Task.Run(Connect);
            }

            return true;
        }

        public bool Inactivate()
        {
            resume = false;

            Abort();

            return true;
        }

        public void PluginClose()
        {
            Abort();
        }

        public void PluginLoad()
        {

        }

        public bool IsSupport(string url)
        {
            userId = url.RegexString(@"https://www.twitch.tv/(?<value>[\w]+)", "value");

            if (!userId.Equals(""))
                return true;

            return false;
        }

        public void ShowWindow(Window window)
        {
            window.Title = "Twitch設定";
            window.Closed += (_, _) => setting.Save();
            window.Content = new SettingPage();
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        public async void Connect()
        {
            while (resume)
            {
                Start("irc.twitch.tv", 6667, "mcc_twitch_bot", setting.Password, userId);

                await Task.Delay(10000);
            }
        }

        protected override async void Process(StreamReader reader)
        {
            try
            {
                while (Connected)
                {
                    var message = await reader.ReadLineAsync();

                    if (message != null)
                    {
                        if (message.Contains("PRIVMSG"))
                        {
                            var intIndexParseSign = message.IndexOf('!');
                            var userName = message.Substring(1, intIndexParseSign - 1);
                            intIndexParseSign = message.IndexOf(" :");
                            message = message.Substring(intIndexParseSign + 2);

                            var commentData = new CommentData()
                            {
                                LiveName = "Twitch",
                                PostTime = DateTime.Now,
                                Comment = message,
                                UserName = "",
                                UserID = userName
                            };

                            OnCommentReceived?.Invoke(this, new(commentData));
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
                Abort();
            }
        }
    }
}

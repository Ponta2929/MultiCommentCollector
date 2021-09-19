using MCC.Plugin;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MCC.Twitch
{
    public class Twitch : IPluginSender, ILogged, ISetting
    {
        private Setting setting = Setting.Instance;
        private TwitchConnector connector = new();
        private Hashtable table = new();

        public string SiteName => "Twitch";

        public string Author => "ぽんた";

        public string PluginName => "Twitch";

        public string StreamKey { get; set; }

        public string Description => "Twitchのコメントを取得します。";

        public string Version => "1.0.0";

        public string MenuItemName => "設定";

        public event CommentReceivedEventHandler OnCommentReceived;
        public event LoggedEventHandler OnLogged;


        public bool Activate()
        {
            connector.Resume = true;

            Task.Run(() => connector.Connect(setting.Password, StreamKey));

            return true;
        }

        public bool Inactivate()
        {
            connector.Resume = false;

            connector.Abort();

            return true;
        }

        public void PluginClose() => Inactivate();

        public void PluginLoad()
        {
            connector.OnLogged += BaseLogged;
            connector.OnReceived += OnReceived;

            table["Streamer"] = new AdditionalData() { Data = "Streamer", Description = "Streamer" };
        }

        private void OnReceived(object sender, ChatReceivedEventArgs e)
        {
            var list = new List<AdditionalData>();
            var intIndexParseSign = e.ReceiveData.IndexOf('!');
            var userName = e.ReceiveData.Substring(1, intIndexParseSign - 1);
            intIndexParseSign = e.ReceiveData.IndexOf("#");
            var streamer = e.ReceiveData.Substring(intIndexParseSign + 1, e.ReceiveData.IndexOf(" :") - intIndexParseSign - 1);
            intIndexParseSign = e.ReceiveData.IndexOf(" :");
            var comment = e.ReceiveData.Substring(intIndexParseSign + 2);

            if (userName.Equals(streamer))
            {
                list.Add(table["Streamer"] as AdditionalData);
            }

            var commentData = new CommentData()
            {
                LiveName = "Twitch",
                PostTime = DateTime.Now,
                Comment = comment,
                UserName = string.Empty,
                UserID = userName,
                Additional = list.ToArray()
            };

            OnCommentReceived?.Invoke(this, new(commentData));
        }

        public bool IsSupport(string url)
        {
            StreamKey = url.RegexString(@"https://www.twitch.tv/(?<value>[\w]+)", "value");

            return !string.IsNullOrEmpty(StreamKey);
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

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}

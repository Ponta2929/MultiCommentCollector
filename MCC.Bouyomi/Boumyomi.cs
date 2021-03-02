using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MCC.Plugin;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.IO;
using MCC.Utility.Net;
using MCC.Utility.Text;

namespace MCC.Bouyomi
{
    public class Boumyomi : IPluginReceiver, ISetting
    {
        private Setting setting = Setting.GetInstance();
        private bool resume = false;

        public string Author => "ぽんた";

        public string PluginName => "棒読みちゃん";

        public string Description => "棒読みちゃんにコメントを発言させるプラグインです。";

        public string Version => "1.0.0";

        public string MenuItemName => "設定";

        public void PluginClose()
        {

        }

        public void PluginLoad()
        {

        }

        public void Receive(CommentData comment)
        {
            if (setting.Enable)
            {
                Task.Run(() =>
                {
                    ExecuteProcess();

                    // 読ませる
                    Http.Get($"http://localhost:50080/Talk?text=\"{DataFormat(comment)}\"");
                });
            }
        }

        /// <summary>
        /// 棒読みちゃんが存在しないなら起動
        /// </summary>
        public void ExecuteProcess()
        {
            var process = Process.GetProcessesByName("BouyomiChan");
        
            if (process.Length == 0 && File.Exists(setting.ApplicationPath))
                Process.Start(setting.ApplicationPath);
        }

        /// <summary>
        /// 対応する文字列を補間
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public string DataFormat(CommentData comment)
        {
            var format = setting.Format;
            var regex = format.RegexStrings(@"\$\{(?<value>.*?)\}", "value");

            foreach (var item in regex)
            {
                foreach (var info in comment.GetType().GetProperties())
                {
                    if (info.Name.Equals(item))
                    {
                        format = format.Replace("${" + item + "}", info.GetValue(comment)?.ToString());
                    }
                }
            }

            return format;
        }

        public void ShowWindow(Window window)
        {
            window.Title = "棒読みちゃん設定";
            window.Closed += (_, _) => setting.Save();
            window.Content = new SettingPage();
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }
    }
}

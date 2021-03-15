using MCC.Plugin;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MCC.Bouyomi
{
    public class Boumyomi : IPluginReceiver, ISetting
    {
        private Setting setting = Setting.GetInstance();

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
            foreach (var item in Setting.GetInstance().BlackListItems)
                BlackList.GetInstance().Add(item);
        }

        public void Receive(CommentData comment)
        {
            if (setting.Enable)
            {
                Task.Run(() =>
                {
                    ExecuteProcess();

                    if (IsRead(comment))
                        // 読ませる
                        Http.Get($"http://localhost:50080/Talk?text=\"{DataFormat(comment)}\"");
                });
            }
        }

        public bool IsRead(CommentData comment)
        {
            var blackList = BlackList.GetInstance();

            foreach (var item in blackList)
            {
                var userName = false;
                var userId = false;
                var liveName = false;
                var commente = false;

                if (item.LiveName.Equals("*") && item.UserName.Equals("*") && item.UserID.Equals("*") && item.Comment.Equals("*"))
                    continue;

                if (IsRegex(item.Comment))
                    commente = Regex(item.Comment, comment.Comment);
                else if (item.Comment.Equals("*"))
                    commente = true;
                else
                    commente = comment.Comment.Equals(item.Comment);

                if (IsRegex(item.LiveName))
                    liveName = Regex(item.LiveName, comment.LiveName);
                else if (item.LiveName.Equals("*"))
                    liveName = true;
                else
                    liveName = comment.LiveName.Equals(item.LiveName);

                if (IsRegex(item.UserID))
                    userId = Regex(item.UserID, comment.UserID);
                else if (item.UserID.Equals("*"))
                    userId = true;
                else
                    userId = comment.UserID.Equals(item.UserID);

                if (IsRegex(item.UserName))
                    userName = Regex(item.UserName, comment.UserName);
                else if (item.UserName.Equals("*"))
                    userName = true;
                else
                    userName = comment.UserName.Equals(item.UserName);

                if (userName && userId && liveName && commente)
                    return false;
            }

            return true;
        }

        public bool IsRegex(string target)
        {
            var index = target.IndexOf("Regex(");
            var last = target.LastIndexOf(")");

            if (index != -1 && last != -1)
                return true;

            return false;
        }

        public bool Regex(string target, string message)
        {
            var index = target.IndexOf("Regex(");
            var last = target.LastIndexOf(")");

            if (index != -1 && last != -1)
            {
                var regex = target.Substring(index, last - index).Replace("Regex(", "");

                try
                {
                    var r = new Regex(regex);

                    return r.IsMatch(message);
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 棒読みちゃんが存在しないなら起動
        /// </summary>
        private void ExecuteProcess()
        {
            var process = Process.GetProcessesByName("BouyomiChan");

            if (process.Length == 0 && File.Exists(setting.ApplicationPath))
            {
                Process.Start(setting.ApplicationPath);
            }
        }

        /// <summary>
        /// 対応する文字列を補間
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        private string DataFormat(CommentData comment)
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

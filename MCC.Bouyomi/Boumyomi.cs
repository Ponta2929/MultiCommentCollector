using MCC.Plugin;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.Net;
using MCC.Utility.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace MCC.Bouyomi
{
    public class Boumyomi : IPluginReceiver, ISetting
    {
        private Setting setting = Setting.Instance;

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
            foreach (var item in Setting.Instance.BlackListItems)
                BlackList.Instance.Add(item);
        }

        public void Receive(CommentData comment)
        {
            if (setting.Enable)
            {
                Task.Run(() =>
                {
                    ExecuteProcess();

                    if (setting.BlackListEnable)
                    {
                        if (IsRead(comment))
                        {
                            Http.Get($"http://localhost:50080/Talk?text={HttpUtility.UrlEncode(DataFormat(comment))}");
                        }
                    }
                    else
                    {
                        Http.Get($"http://localhost:50080/Talk?text={HttpUtility.UrlEncode(DataFormat(comment))}");
                    }
                });
            }
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

        public bool IsRead(CommentData comment)
        {
            var blackList = BlackList.Instance;

            foreach (var item in blackList)
            {
                var (userName, userId, liveName, commente) = (false, false, false, false);

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

        /// <summary>
        /// 正規表現文字列かどうか。
        /// </summary>
        /// <param name="target">チェックするテキスト</param>
        /// <returns></returns>
        public bool IsRegex(string target)
        {
            var index = target.IndexOf("Regex(");
            var last = target.LastIndexOf(")");

            if (index != -1 && last != -1)
                return true;

            return false;
        }

        /// <summary>
        /// 正規表現処理を行う。
        /// </summary>
        /// <param name="target">正規表現文字列</param>
        /// <param name="message">正規表現の対象</param>
        /// <returns></returns>
        public bool Regex(string target, string message)
        {
            var index = target.IndexOf("Regex(");
            var last = target.LastIndexOf(")");

            if (index != -1 && last != -1)
            {
                var regex = target.Substring(index, last - index).Replace("Regex(", "");

                try
                {
                    return new Regex(regex).IsMatch(message);
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
    }
}

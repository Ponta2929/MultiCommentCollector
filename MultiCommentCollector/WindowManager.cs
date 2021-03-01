using MahApps.Metro.Controls;
using MCC.Core;
using MCC.Core.Server;
using MCC.Utility.IO;
using MCC.Utility.Text;
using System;
using System.IO;
using System.Windows;

namespace MultiCommentCollector
{
    public static class WindowManager
    {
        public static void ApplicationStart()
        {
            string te = "${test} ${sxxx} ${dxcxzc}";

            var teee = te.RegexStrings(@"\$\{(?<value>.*?)\}", "value");
            foreach (var item in teee)
            {
                te = te.Replace("${" + item + "}", "TEST");
            }
            Setting setting = Setting.GetInstance();

            Application.Current.MainWindow.Closing += ApplicationClosing;

            // 接続リスト
            foreach (var item in Setting.GetInstance().ConnectionList)
                ConnectionManager.GetInstance().Add(item);

            // サーバー開始
            CommentGeneratorServer.GetInstance().Port = setting.Servers.CommentGeneratorServerPort.Value;
            CommentReceiverServer.GetInstance().Port = setting.Servers.CommentReceiverServerPort.Value;
            MCC.Core.Win.MultiCommentCollector.GetInstance().Apply();
            MCC.Core.Win.MultiCommentCollector.GetInstance().ServerStart();
        }

        private static void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MCC.Core.Win.MultiCommentCollector.GetInstance().ServerStop();

            Setting.GetInstance().ConnectionList = ConnectionManager.GetInstance();

            foreach (var item in PluginManager.GetInstance())
                item.PluginClose();

            try
            {
                XmlSerializer.FileSerialize<Setting>($"{Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])}\\setting.xml", Setting.GetInstance());
            }
            catch
            {

            }

            LogWindow.GetInstance().IsOwnerClose = true;
        }

        public static void ShowLogWindow()
            => LogWindow.GetInstance().Show();

        public static void ShowPluginWindow()
        {
            var plugin = new PluginWindow();
            plugin.Owner = Application.Current.MainWindow;
            plugin.ShowDialog();
        }

        public static void ShowOptionWindow()
        {
            var option = new OptionWindow();
            option.Owner = Application.Current.MainWindow;
            option.ShowDialog();
        }

        public static void ApplicationShutdown()
            => Application.Current.MainWindow.Close();
    }
}

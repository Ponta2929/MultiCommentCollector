using MCC.Core;
using MCC.Utility.IO;
using System;
using System.IO;
using System.Windows;

namespace MultiCommentCollector
{
    public static class WindowManager
    {
        public static void ApplicationStart()
        {
            Setting setting = Setting.GetInstance();

            Application.Current.MainWindow.Closing += ApplicationClosing;

            // 接続リスト
            foreach (var item in Setting.GetInstance().ConnectionList)
                ConnectionManager.GetInstance().Add(item);

            // サーバー開始
            CommentGeneratorServer.GetInstance().Port = setting.Servers.CommentGeneratorServerPort.Value;
            CommentReceiverServer.GetInstance().Port = setting.Servers.CommentReceiverServerPort.Value;
            MCC.Core.MultiCommentCollector.GetInstance().Apply();
            MCC.Core.MultiCommentCollector.GetInstance().ServerStart();
        }

        private static void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MCC.Core.MultiCommentCollector.GetInstance().ServerStop();

            Setting.GetInstance().ConnectionList = ConnectionManager.GetInstance();

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

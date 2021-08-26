using MahApps.Metro.Controls;
using MCC.Core;
using MCC.Core.Manager;
using MCC.Core.Server;
using MCC.Utility;
using MCC.Utility.IO;
using MCC.Utility.Text;
using Reactive.Bindings.Notifiers;
using System;
using System.IO;
using System.Windows;

namespace MultiCommentCollector
{
    public static class WindowManager
    {
        public static void ApplicationStart()
        {
            var setting = Setting.GetInstance();

            Application.Current.MainWindow.Closing += ApplicationClosing;

            // 接続リスト
            foreach (var item in setting.ConnectionList)
                ConnectionManager.GetInstance().Add(item);

            foreach (var item in UserSetting.GetInstance().UserDataList)
                UserDataManager.GetInstance().Add(item);

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

            Utility.SaveToXml<Setting>("setting.xml", Setting.GetInstance());

            LogWindow.GetInstance().IsOwnerClose = true;
        }

        public static void ShowLogWindow()
        {
            LogWindow.GetInstance().Owner = Application.Current.MainWindow;
            LogWindow.GetInstance().Show();
            LogWindow.GetInstance().Activate();
        }

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

        public static void ShowUserDataWindow(UserData user)
        {
            var userData = new UserDataWindow();
            MessageBroker.Default.Publish<UserData>(user);
            userData.Owner = Application.Current.MainWindow;
            userData.ShowDialog();
        }

        public static void ShowUsersSettingWindow()
        {
            UsersSettingWindow.GetInstance().Owner = Application.Current.MainWindow;
            UsersSettingWindow.GetInstance().Show();
            UsersSettingWindow.GetInstance().Activate();
        }
        

        public static void CloseWindow<T>() where T : Window
        {
            foreach (var window in Application.Current.Windows)
                if (window is T t)
                    t.Close();
        }

        public static void CloseWindow(object viewModel)
        {
            foreach (var window in Application.Current.Windows)
                if (((Window)window).DataContext is not null && ((Window)window).DataContext.Equals(viewModel))
                    ((Window)window).Close();
        }

        public static void ApplicationShutdown()
            => Application.Current.MainWindow.Close();
    }
}

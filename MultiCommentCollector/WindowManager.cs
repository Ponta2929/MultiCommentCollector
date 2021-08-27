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
            var setting = Setting.Instance;

            Application.Current.MainWindow.Closing += ApplicationClosing;

            // 接続リスト
            foreach (var item in setting.ConnectionList)
                ConnectionManager.Instance.Add(item);

            foreach (var item in UserSetting.Instance.UserDataList)
                UserDataManager.Instance.Add(item);

            // サーバー開始
            CommentGeneratorServer.Instance.Port = setting.Servers.CommentGeneratorServerPort.Value;
            CommentReceiverServer.Instance.Port = setting.Servers.CommentReceiverServerPort.Value;
            MCC.Core.Win.MultiCommentCollector.Instance.Apply();
            MCC.Core.Win.MultiCommentCollector.Instance.ServerStart();
        }

        private static void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MCC.Core.Win.MultiCommentCollector.Instance.ServerStop();

            Setting.Instance.ConnectionList = ConnectionManager.Instance;

            foreach (var item in PluginManager.Instance)
                item.PluginClose();

            Utility.SaveToXml<Setting>("setting.xml", Setting.Instance);

            LogWindow.Instance.IsOwnerClose = true;
        }

        public static void ShowLogWindow()
        {
            LogWindow.Instance.Owner = Application.Current.MainWindow;
            LogWindow.Instance.Show();
            LogWindow.Instance.Activate();
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

        public static void ShowUserSettingWindow(UserData user)
        {
            var userData = new UserSettingWindow();
            MessageBroker.Default.Publish<UserData>(user);
            userData.Owner = Application.Current.MainWindow;
            userData.ShowDialog();
        }

        public static void ShowUsersSettingWindow()
        {
            UsersSettingWindow.Instance.Owner = Application.Current.MainWindow;
            UsersSettingWindow.Instance.Show();
            UsersSettingWindow.Instance.Activate();
        }

        public static void ShowUserDataWindow(CommentDataEx user)
        {
            var userData = new UserDataWindow();
            userData.CreateViewModel(user);
            userData.Owner = Application.Current.MainWindow;
            userData.Show();
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
            {
                var target = window as Window;

                if (target.DataContext is not null && target.DataContext.Equals(viewModel))
                    target.Close();
            }
        }

        public static void ApplicationShutdown()
            => Application.Current.MainWindow.Close();
    }
}

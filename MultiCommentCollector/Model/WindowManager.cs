using MCC.Core.Manager;
using MCC.Core.Server;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.View;
using MultiCommentCollector.ViewModel;
using System.Linq;
using System.Windows;

namespace MultiCommentCollector.Model
{
    public static class WindowManager
    {
        private static MCC.Core.Win.MultiCommentCollector mcc = MCC.Core.Win.MultiCommentCollector.Instance;
        private static CommentReceiverServer receiverServer = CommentReceiverServer.Instance;
        private static CommentGeneratorServer generatorServer = CommentGeneratorServer.Instance;
        private static ConnectionManager connectionManager = ConnectionManager.Instance;
        private static CommentManager commentManager = CommentManager.Instance;
        private static PluginManager pluginManager = PluginManager.Instance;
        private static LogManager logManager = LogManager.Instance;
        private static UserDataManager userDataManager = UserDataManager.Instance;
        private static UserSetting userSetting = UserSetting.Instance;
        private static Setting setting = Setting.Instance;
        private static LogWindow logWindow = LogWindow.Instance;
        private static UsersSettingWindow usersSettingWindow = UsersSettingWindow.Instance;

        public static void ApplicationStart()
        {
            Application.Current.MainWindow.Closing += ApplicationClosing;

            // 接続リスト
            foreach (var item in setting.ConnectionList)
                connectionManager.Add(item);

            foreach (var item in userSetting.UserDataList)
                userDataManager.Add(item);

            // サーバー開始
            generatorServer.Port = setting.Servers.CommentGeneratorServerPort.Value;
            receiverServer.Port = setting.Servers.CommentReceiverServerPort.Value;
            mcc.Apply();
            mcc.ServerStart();
        }

        private static void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mcc.ServerStop();

            setting.ConnectionList = connectionManager;

            foreach (var item in pluginManager)
                item.PluginClose();

            SerializeHelper.SaveToXml<Setting>("setting.xml", Setting.Instance);
        }

        public static void ShowLogWindow()
        {
            logWindow.Owner = Application.Current.MainWindow;
            logWindow.Show();
            logWindow.Activate();
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

        public static void ShowUserSettingWindow(CommentDataEx commentData)
        {
            if (commentData is not null)
            {
                var usersData = userDataManager.FirstOrDefault(x => x.LiveName.Equals(commentData.LiveName) && x.UserID.Equals(commentData.UserID));

                // ウィンドウ表示
                ShowUserSettingWindow(usersData ?? new(commentData));
            }
        }

        public static void ShowUserSettingWindow(UserData user)
        {
            if (user is null)
                return;

            var userData = new UserSettingWindow();
            userData.DataContext = new UserSettingWindowViewModel(user);
            userData.Owner = Application.Current.MainWindow;
            userData.ShowDialog();
        }

        public static void ShowUsersSettingWindow()
        {
            usersSettingWindow.Owner = Application.Current.MainWindow;
            usersSettingWindow.Show();
            usersSettingWindow.Activate();
        }

        public static void ShowUserDataWindow(CommentDataEx user)
        {
            if (user is null)
                return;

            var userData = new UserDataWindow();
            userData.DataContext = new UserDataWindowViewModel(user);
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

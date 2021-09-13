using MCC.Core.Manager;
using MCC.Core.Server;
using MCC.Utility;
using MultiCommentCollector.Helper;
using MultiCommentCollector.ViewModels;
using MultiCommentCollector.Views;
using System.Linq;
using System.Windows;

namespace MultiCommentCollector.Models
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
        private static Setting setting = Setting.Instance;
        private static LogWindow logWindow = LogWindow.Instance;
        private static UsersSettingWindow usersSettingWindow = UsersSettingWindow.Instance;

        public static void ShowLogWindow()
        {
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
            {
                return;
            }

            var userData = new UserSettingWindow();
            userData.DataContext = new UserSettingWindowViewModel(user);
            userData.Owner = Application.Current.MainWindow;
            userData.ShowDialog();
        }

        public static void ShowUsersSettingWindow()
        {
            usersSettingWindow.Show();
            usersSettingWindow.Activate();
        }

        public static void ShowUserDataWindow(CommentDataEx user)
        {
            if (user is null || WindowHelper.SearchUserDataWindow(user.LiveName, user.UserID))
            {
                return;
            }

            var userData = new UserDataWindow();
            userData.DataContext = new UserDataWindowViewModel(user);
            userData.Show();
            userData.Activate();
        }

        public static void ApplicationShutdown()
            => Application.Current.MainWindow.Close();
    }
}

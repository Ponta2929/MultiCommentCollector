using MCC.Core.Manager;
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
        public static void ShowLogWindow()
        {
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

        public static void ShowUserSettingWindow(CommentDataEx commentData)
        {
            if (commentData is not null)
            {
                var usersData = UserDataManager.Instance.FirstOrDefault(x => x.LiveName.Equals(commentData.LiveName) && x.UserID.Equals(commentData.UserID));

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
            UsersSettingWindow.Instance.Show();
            UsersSettingWindow.Instance.Activate();
        }

        public static void ShowUserDataWindow(CommentDataEx user)
        {
            if (user is null || WindowHelper.SearchUserDataWindow(user.LiveName, user.UserID))
            {
                return;
            }

            var userData = new UserDataWindow();
            userData.DataContext = new UserDataWindowViewModel(user);
            userData.Owner = Application.Current.MainWindow;
            userData.Show();
            userData.Activate();
        }

        public static void ApplicationShutdown() => Application.Current.MainWindow.Close();
    }
}

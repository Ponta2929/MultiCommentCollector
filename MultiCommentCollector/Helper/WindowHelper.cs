using MultiCommentCollector.ViewModels;
using System.Windows;

namespace MultiCommentCollector.Helper
{
    public static class WindowHelper
    {
        /// <summary>
        /// ユーザーデータウィンドウを検索します。
        /// </summary>
        /// <param name="liveName">キー:配信サイト名</param>
        /// <param name="userId">キー:ユーザーID</param>
        /// <returns>すでにある場合はTrue、それ以外はFalse</returns>
        public static bool SearchUserDataWindow(string liveName, string userId)
        {
            foreach (var window in Application.Current.Windows)
            {
                var target = window as Window;

                if (target.DataContext is not null && target.DataContext is UserDataWindowViewModel user)
                {
                    if (user.LiveName == liveName && user.UserId == userId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 対象のViewModeを持つウィンドウを検索します。
        /// </summary>
        /// <param name="viewModel">検索対象のViewModel</param>
        /// <returns>見つかった場合はWindow、それ以外はnull</returns>
        public static Window SearchWindow(object viewModel)
        {
            foreach (var window in Application.Current.Windows)
            {
                var target = window as Window;

                if (target.DataContext is not null && target.DataContext.Equals(viewModel))
                {
                    return target;
                }
            }

            return null;
        }

        /// <summary>
        /// 対象のViewModeを持つウィンドウを終了させます。
        /// </summary>
        /// <param name="viewModel">終了させるウィンドウが持つViewModel</param>
        public static void CloseWindow(object viewModel)
        {
            foreach (var window in Application.Current.Windows)
            {
                var target = window as Window;

                if (target.DataContext is not null && target.DataContext.Equals(viewModel))
                {
                    target.Close();
                }
            }
        }

    }
}

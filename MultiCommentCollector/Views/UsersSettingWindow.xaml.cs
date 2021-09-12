using MultiCommentCollector.Models;

namespace MultiCommentCollector.Views
{
    /// <summary>
    /// UsersSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UsersSettingWindow : SingleMetroWindow
    {
        #region Singleton

        private static UsersSettingWindow instance;
        public static UsersSettingWindow Instance => instance ??= new();

        #endregion

        public UsersSettingWindow()
        {
            InitializeComponent();
        }
    }
}
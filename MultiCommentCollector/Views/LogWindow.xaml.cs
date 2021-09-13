using MultiCommentCollector.Models;

namespace MultiCommentCollector.Views
{
    /// <summary>
    /// LogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LogWindow : SingleMetroWindow
    {
        #region Singleton

        private static LogWindow instance;
        public static LogWindow Instance => instance ??= new();

        #endregion

        public LogWindow() => InitializeComponent();
    }
}

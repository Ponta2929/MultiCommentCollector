using MultiCommentCollector.Model;

namespace MultiCommentCollector.View
{
    /// <summary>
    /// LogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LogWindow : SingleMetroWindowBase
    {
        #region Singleton

        private static LogWindow instance;
        public static LogWindow Instance => instance ??= new();

        #endregion

        public LogWindow()
        {
            InitializeComponent();
        }
    }
}

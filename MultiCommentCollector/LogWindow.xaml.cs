using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using MCC.Core;
using MCC.Core.Manager;
using MCC.Utility;

namespace MultiCommentCollector
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

            // バインド
            LogListView.ItemsSource = LogManager.Instance;
        }
    }
}

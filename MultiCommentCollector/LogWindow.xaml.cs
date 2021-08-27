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
    public partial class LogWindow : MahApps.Metro.Controls.MetroWindow
    {
        #region Singleton

        private static LogWindow instance;
        public static LogWindow Instance => instance ??= new();

        #endregion

        private bool _IsOwnerClose;

        public bool IsOwnerClose
        {
            get => _IsOwnerClose;
            set
            {
                if (_IsOwnerClose = value)
                    Close();
            }
        }

        public LogWindow()
        {
            InitializeComponent();

            // バインド
            LogListView.ItemsSource = LogManager.Instance;
        }

        private void LogWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !IsOwnerClose;
            Visibility = Visibility.Hidden;
        }
    }
}

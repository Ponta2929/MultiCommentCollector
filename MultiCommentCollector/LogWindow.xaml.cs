using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using MCC.Core;

namespace MultiCommentCollector
{
    /// <summary>
    /// LogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LogWindow : MahApps.Metro.Controls.MetroWindow
    {
        #region Singleton

        private static LogWindow instance;
        public static LogWindow GetInstance() => instance ?? (instance = new());
        public static void SetInstance(LogWindow inst) => instance = inst;

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
            LogListView.ItemsSource = LogManager.GetInstance().Items;
        }


        private void LogWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                LogListView.ScrollIntoView(LogListView.Items[e.NewStartingIndex]);
            }
        }

        private void LogWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !IsOwnerClose;
            Visibility = Visibility.Hidden;
        }
    }
}

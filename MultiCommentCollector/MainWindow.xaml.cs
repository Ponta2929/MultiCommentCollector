using MCC.Core.Manager;

namespace MultiCommentCollector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // バインド
            // CommentListView.ItemsSource = CommentManager.Instance;
            ConnectionListView.ItemsSource = ConnectionManager.Instance;
        }
    }
}

using MCC.Core;
using System.Windows;

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
            CommentListView.ItemsSource = CommentManager.GetInstance();
            ConnectionListView.ItemsSource = ConnectionManager.GetInstance();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MCC.Core.Win.MultiCommentCollector.GetInstance().TestA(new MahApps.Metro.Controls.MetroWindow());
        }
    }
}

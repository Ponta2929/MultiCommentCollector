using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows;

namespace MultiCommentCollector.Models
{
    public class SingleMetroWindow : MetroWindow
    {
        private bool _IsOwnerClose;

        public bool IsOwnerClose
        {
            get => _IsOwnerClose;
            set
            {
                if (_IsOwnerClose = value)
                {
                    Close();
                }
            }
        }

        public SingleMetroWindow()
        {
            Application.Current.MainWindow.Closing += (_, _)
                => IsOwnerClose = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !IsOwnerClose;

            Visibility = Visibility.Hidden;
        }
    }
}

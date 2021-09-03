using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows;

namespace MultiCommentCollector.Model
{
    public class SingleMetroWindowBase : MetroWindow
    {
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

        public SingleMetroWindowBase()
        {
            Application.Current.MainWindow.Closing += (sender, e) =>
            {
                IsOwnerClose = true;
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !IsOwnerClose;

            Visibility = Visibility.Hidden;
        }
    }
}

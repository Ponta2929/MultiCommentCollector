using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiCommentCollector
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

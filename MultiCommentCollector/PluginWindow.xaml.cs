using MCC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiCommentCollector
{
    /// <summary>
    /// PluginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginWindow : MahApps.Metro.Controls.MetroWindow
    {
        #region Singleton

        private static PluginWindow instance;
        public static PluginWindow GetInstance() => instance ??= new();
        public static void SetInstance(PluginWindow inst) => instance = inst;

        #endregion

        public PluginWindow()
        {
            InitializeComponent();

            // バインド
            PluginList.ItemsSource = PluginManager.GetInstance();
        }
    }
}

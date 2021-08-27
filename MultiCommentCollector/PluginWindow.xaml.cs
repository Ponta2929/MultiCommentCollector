using MCC.Core;
using MCC.Core.Manager;

namespace MultiCommentCollector
{
    /// <summary>
    /// PluginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginWindow : MahApps.Metro.Controls.MetroWindow
    {
        #region Singleton

        private static PluginWindow instance;
        public static PluginWindow Instance => instance ??= new();

        #endregion

        public PluginWindow()
        {
            InitializeComponent();

            // バインド
            PluginList.ItemsSource = PluginManager.Instance.Parent;
        }
    }
}

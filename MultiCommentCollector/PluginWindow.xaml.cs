using MCC.Core;

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
            PluginList.ItemsSource = PluginManager.GetInstance().Parent;
        }
    }
}

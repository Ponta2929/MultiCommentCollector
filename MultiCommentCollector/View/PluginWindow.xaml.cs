namespace MultiCommentCollector.View
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
        }
    }
}

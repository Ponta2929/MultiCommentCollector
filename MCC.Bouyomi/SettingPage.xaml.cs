using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace MCC.Bouyomi
{
    /// <summary>
    /// SettingPage.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();

            // 配列設定
            DataGrid_BlackList.ItemsSource = BlackList.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "BouyomiChan.exe|BouyomiChan.exe";

            if (dialog.ShowDialog() == true)
            {
                FilePath.Text = Setting.Instance.ApplicationPath = dialog.FileName;
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_BlackList.SelectedItem is BlackListItem item)
            {
                BlackList.Instance.Remove(item);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // 配列設定
            DataGrid_BlackList.ItemsSource = null;
        }
    }
}

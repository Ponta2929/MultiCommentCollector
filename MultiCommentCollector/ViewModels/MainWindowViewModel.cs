using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using MCC.Core.Manager;
using MCC.Plugin.Win;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private WindowRect window = Setting.Instance.MainWindow;
        private Pane pane = Setting.Instance.Pane;
        private Models.Theme theme = Setting.Instance.Theme;

        private MCC.Core.Win.MultiCommentCollector mcc = MCC.Core.Win.MultiCommentCollector.Instance;
        private PluginManager pluginManager = PluginManager.Instance;

        public ReactiveProperty<double> Width { get; }
        public ReactiveProperty<double> Height { get; }
        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<bool> IsPaneOpen { get; }
        public ReactiveProperty<int> PaneWidth { get; }

        public ReactiveCommand ShowLogWindowCommand { get; }
        public ReactiveCommand ShowPluginWindowCommand { get; }
        public ReactiveCommand ShowOptionWindowCommand { get; }
        public ReactiveCommand ShowUsersSettingWindowCommand { get; }
        public ReactiveCommand ApplicationShutdownCommand { get; }
        public ReactiveCommand<string> EnterCommand { get; }
        public ReactiveCommand<ConnectionData> ActivateCommand { get; }
        public ReactiveCommand<ConnectionData> InactivateCommand { get; }
        public ReactiveCommand<ConnectionData> DeleteCommand { get; }
        public ReactiveCommand<ConnectionData> ToggleCommand { get; }
        public ReactiveCommand<RoutedEventArgs> ColumnHeaderUserClickCommand { get; }
        public ReactiveCommand<RoutedEventArgs> ColumnHeaderConnectionClickCommand { get; }
        public ReactiveCommand<RoutedEventArgs> CellItemUserClickCommand { get; }
        public ReactiveCommand<object> ShowUserSettingCommand { get; }
        public ReactiveCommand<object> ShowUserDataCommand { get; }
        public ReactiveCommand<MenuItem> MenuItemOpenedCommand { get; }
        public ReactiveCollection<MenuItem> ParentMenuPlugins { get; }

        public CollectionViewSource CommentFilterView { get; }
        public CollectionViewSource ConnectionView { get; }

        public MainWindowViewModel()
        {
            // MainWindow
            Width = window.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            Height = window.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            Left = window.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            Top = window.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            IsPaneOpen = pane.IsPaneOpen.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            PaneWidth = pane.PaneWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);

            // Commands
            ShowLogWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowLogWindow).AddTo(Disposable);
            ShowPluginWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowPluginWindow).AddTo(Disposable);
            ShowOptionWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowOptionWindow).AddTo(Disposable);
            ShowUsersSettingWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowUsersSettingWindow).AddTo(Disposable);
            ApplicationShutdownCommand = new ReactiveCommand().WithSubscribe(WindowManager.ApplicationShutdown).AddTo(Disposable);
            EnterCommand = new ReactiveCommand<string>().WithSubscribe(x => mcc.AddURL(x)).AddTo(Disposable);
            ActivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => mcc.Activate(x)).AddTo(Disposable);
            InactivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(mcc.Inactivate).AddTo(Disposable);
            DeleteCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(mcc.RemoveConnection).AddTo(Disposable);
            ToggleCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(mcc.ToggleConnection).AddTo(Disposable);
            ColumnHeaderUserClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(UserHeader_Click).AddTo(Disposable);
            ColumnHeaderConnectionClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(ConnectionHeader_Click).AddTo(Disposable);
            CellItemUserClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(x => WindowManager.ShowUserDataWindow((x.Source as PackIconMaterial).DataContext as CommentDataEx)).AddTo(Disposable);
            ShowUserSettingCommand = new ReactiveCommand<object>().WithSubscribe(x => WindowManager.ShowUserSettingWindow(x as CommentDataEx)).AddTo(Disposable);
            ShowUserDataCommand = new ReactiveCommand<object>().WithSubscribe(x => WindowManager.ShowUserDataWindow(x as CommentDataEx)).AddTo(Disposable);
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(MenuItemCopy_Opened).AddTo(Disposable);

            // フィルター            
            ConnectionView = new() { Source = ConnectionManager.Instance };
            CommentFilterView = new() { Source = CommentManager.Instance };
            CommentFilterView.Filter += CommentFilter_Filter;

            // Theme
            theme.IsDarkMode.Subscribe(x => ThemeHelper.ThemeChange(theme.ThemeColor.Value.ToArgb(), x)).AddTo(Disposable);
            theme.ThemeColor.Subscribe(x => ThemeHelper.ThemeChange(x.ToArgb(), theme.IsDarkMode.Value)).AddTo(Disposable);

            // プラグインメニュー作成
            ParentMenuPlugins = new ReactiveCollection<MenuItem>().AddTo(Disposable);
            CreateMenuItemPlugins();

            // コメントフィルタリング
            MessageBroker.Default.Subscribe<MessageArgs>(x => { if (x.Identifier is "Refresh.Comment.View") CommentFilterView.View.Refresh(); }).AddTo(Disposable);
        }

        private void CommentFilter_Filter(object _, FilterEventArgs e)
        {
            var item = e.Item as CommentDataEx;
            var userData = UserDataManager.Instance.Find(item);

            if (userData is not null && userData.HideUser)
                e.Accepted = false;
            else
                e.Accepted = true;
        }

        /// <summary>
        /// ユーザーID/ユーザー名ヘッダークリック
        /// </summary>
        private void UserHeader_Click(RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader header)
            {
                if (header.Content.Equals("ユーザー名"))
                {
                    header.Content = "ユーザーID";
                    header.Column.CellTemplate = Application.Current.Resources["GridViewCellItemUserID"] as DataTemplate;
                }
                else if (header.Content.Equals("ユーザーID"))
                {
                    header.Content = "ユーザー名";
                    header.Column.CellTemplate = Application.Current.Resources["GridViewCellItemUserName"] as DataTemplate;
                }
            }
        }

        /// <summary>
        /// URL/StreamKeyヘッダークリック
        /// </summary>
        private void ConnectionHeader_Click(RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader header)
            {
                if (header.Content.Equals("接続先 / URL"))
                {
                    header.Content = "接続先 / ストリームキー";
                    header.Column.CellTemplate = Application.Current.Resources["GridViewCellItemStreamKey"] as DataTemplate;
                }
                else if (header.Content.Equals("接続先 / ストリームキー"))
                {
                    header.Content = "接続先 / URL";
                    header.Column.CellTemplate = Application.Current.Resources["GridViewCellItemURL"] as DataTemplate;
                }
            }
        }

        /// <summary>
        /// プラグインのメニューリストを作成
        /// </summary>
        private void CreateMenuItemPlugins()
        {
            var parent = pluginManager.Parent.Select(x => new MenuItem() { Header = x.PluginName });

            foreach (var item in parent)
            {
                item.ItemsSource = pluginManager.Parent.Where(x => x.PluginName.Equals(item.Header) && x is ISetting);
                item.DisplayMemberPath = "MenuItemName";
                item.Click += MeunItemSetting_Click;

                // 追加
                ParentMenuPlugins.Add(item);
            }
        }

        private void MeunItemSetting_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.Items is not null && item.Items.Count > 0)
            {
                var window = new MetroWindow() { Owner = Application.Current.MainWindow };

                (item.Items[0] as ISetting).ShowWindow(window);

               Helper.TabControlHelper.SetHeaderFontSize(window, 14);
            }
        }

        /// <summary>
        /// 項目右クリック時の子メニューを作成
        /// </summary>
        private void MenuItemCopy_Opened(MenuItem menuItem)
        {
            var owner = menuItem.GetParentObject().FindAncestor<ContextMenu>();
            var commentData = (owner.PlacementTarget as ListViewItem).Content as CommentDataEx;

            if (commentData is not null)
            {
                menuItem.Items.Clear();

                // コピー項目設定
                MenuItemHelper.CreateMenuItemToCopy(menuItem, commentData.LiveName);
                MenuItemHelper.CreateMenuItemToCopy(menuItem, commentData.PostTime.ToString("HH:mm:ss"));
                MenuItemHelper.CreateMenuItemToCopy(menuItem, commentData.UserID);
                MenuItemHelper.CreateMenuItemToCopy(menuItem, commentData.UserName);
                MenuItemHelper.CreateMenuItemToCopy(menuItem, commentData.Comment);

                // コメントデータからURL抽出
                MenuItemHelper.CreateMenuItemToCopyURL(menuItem, commentData.Comment);
            }
        }
    }
}

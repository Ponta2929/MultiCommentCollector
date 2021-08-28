using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MCC.Core.Manager;
using MCC.Plugin.Win;
using MCC.Utility;
using MultiCommentCollector.Helper;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiCommentCollector
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        private readonly CompositeDisposable disposable = new();

        public void Dispose()
        {
            disposable.Clear();
            disposable.Dispose();
        }

        private Setting setting = Setting.Instance;
        private MCC.Core.Win.MultiCommentCollector mcc = MCC.Core.Win.MultiCommentCollector.Instance;
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
        public ReactiveCommand<RoutedEventArgs> ColumnHeaderClickCommand { get; }
        public ReactiveCommand<object> ShowUserSettingCommand { get; }
        public ReactiveCommand<object> ShowUserDataCommand { get; }
        public ReactiveCommand<MenuItem> MenuItemOpenedCommand { get; }
        public ReactiveCollection<MenuItem> ParentMenuPlugins { get; }

        public MainWindowViewModel()
        {
            WindowManager.ApplicationStart();

            // MainWindow
            Width = setting.MainWindow.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Height = setting.MainWindow.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Left = setting.MainWindow.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Top = setting.MainWindow.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            IsPaneOpen = setting.IsPaneOpen.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            PaneWidth = setting.PaneWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            // Commands
            ShowLogWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowLogWindow).AddTo(disposable);
            ShowPluginWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowPluginWindow).AddTo(disposable);
            ShowOptionWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowOptionWindow).AddTo(disposable);
            ShowUsersSettingWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowUsersSettingWindow).AddTo(disposable);
            ApplicationShutdownCommand = new ReactiveCommand().WithSubscribe(WindowManager.ApplicationShutdown).AddTo(disposable);
            EnterCommand = new ReactiveCommand<string>().WithSubscribe(x => mcc.AddURL(x)).AddTo(disposable);
            ActivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => mcc.Activate(x)).AddTo(disposable);
            InactivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(mcc.Inactivate).AddTo(disposable);
            DeleteCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(mcc.RemoveConnection).AddTo(disposable);
            ToggleCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(mcc.ToggleConnection).AddTo(disposable);
            ColumnHeaderClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(UserHeader_Click).AddTo(disposable);
            ShowUserSettingCommand = new ReactiveCommand<object>().WithSubscribe(x => WindowManager.ShowUserSettingWindow(x as CommentDataEx)).AddTo(disposable);
            ShowUserDataCommand = new ReactiveCommand<object>().WithSubscribe(x => { if (x is CommentDataEx commentData) WindowManager.ShowUserDataWindow(commentData); }).AddTo(disposable);
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(MenuItemCopy_Opened).AddTo(disposable);

            // Theme
            setting.Theme.IsDarkMode.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(x ? "Dark" : "Light")}.{setting.Theme.ThemeColor.Value}")).AddTo(disposable);
            setting.Theme.ThemeColor.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(setting.Theme.IsDarkMode.Value ? "Dark" : "Light")}.{x}")).AddTo(disposable);

            // プラグインメニュー作成
            ParentMenuPlugins = new ReactiveCollection<MenuItem>().AddTo(disposable);
            AddMenuItemPlugins();
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
                    header.Column.DisplayMemberBinding = new Binding("UserID");
                }
                else if (header.Content.Equals("ユーザーID"))
                {
                    header.Content = "ユーザー名";
                    header.Column.DisplayMemberBinding = new Binding("UserName");
                }
            }
        }

        /// <summary>
        /// プラグインのメニューリストを作成
        /// </summary>
        private void AddMenuItemPlugins()
        {
            var pluginManager = PluginManager.Instance;
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

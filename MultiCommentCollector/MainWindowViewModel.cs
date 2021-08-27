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
using System.Text.RegularExpressions;
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
            EnterCommand = new ReactiveCommand<string>().WithSubscribe(x => MCC.Core.Win.MultiCommentCollector.Instance.AddURL(x)).AddTo(disposable);
            ActivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => MCC.Core.Win.MultiCommentCollector.Instance.Activate(x)).AddTo(disposable);
            InactivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(MCC.Core.Win.MultiCommentCollector.Instance.Inactivate).AddTo(disposable);
            DeleteCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(RemoveConnection).AddTo(disposable);
            ToggleCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(ToggleConnection).AddTo(disposable);
            ColumnHeaderClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(UserHeader_Click).AddTo(disposable);
            ShowUserSettingCommand = new ReactiveCommand<object>().WithSubscribe(ShowUserSetting).AddTo(disposable);
            ShowUserDataCommand = new ReactiveCommand<object>().WithSubscribe(x => { if (x is CommentDataEx commentData) WindowManager.ShowUserDataWindow(commentData); }).AddTo(disposable);
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(MenuItemCopyOpened).AddTo(disposable);

            // Theme
            setting.Theme.IsDarkMode.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(x ? "Dark" : "Light")}.{setting.Theme.ThemeColor.Value}")).AddTo(disposable);
            setting.Theme.ThemeColor.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(setting.Theme.IsDarkMode.Value ? "Dark" : "Light")}.{x}")).AddTo(disposable);

            // プラグインメニュー作成
            ParentMenuPlugins = new ReactiveCollection<MenuItem>().AddTo(disposable);
            AddMenuItemPlugins(); 
        }

        /// <summary>
        /// 接続状況を切り替え
        /// </summary>
        private void ToggleConnection(ConnectionData connection)
        {
            if (connection is null)
                return;

            if (connection.IsActive.Value)
                MCC.Core.Win.MultiCommentCollector.Instance.Inactivate(connection);
            else
                MCC.Core.Win.MultiCommentCollector.Instance.Activate(connection);
        }

        /// <summary>
        /// 接続を削除
        /// </summary>
        /// <param name="connection"></param>
        private void RemoveConnection(ConnectionData connection)
        {
            MCC.Core.Win.MultiCommentCollector.Instance.Inactivate(connection);
            PluginManager.Instance.Remove(connection.Plugin);
            ConnectionManager.Instance.Remove(connection);
        }

        /// <summary>
        /// ユーザーID/ユーザー名ヘッダークリック
        /// </summary>
        /// <param name="e"></param>
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

        private void MeunItemSetting_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item.Items is not null && item.Items.Count > 0)
            {
                var window = new MetroWindow() { Owner = Application.Current.MainWindow };

                (item.Items[0] as ISetting).ShowWindow(window);

                SetHeaderFontSize(window);
            }
        }

        /// <summary>
        /// 子ウィンドウのヘッダーサイズを設定
        /// </summary>
        private void SetHeaderFontSize(DependencyObject element)
        {
            if (element is null)
                return;

            foreach (var child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is DependencyObject control)
                {
                    if (control is TabControl tabControl)
                    {
                        HeaderedControlHelper.SetHeaderFontSize(tabControl, 14);
                    }

                    SetHeaderFontSize(child as DependencyObject);
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

        /// <summary>
        /// ユーザー設定ウィンドウを表示
        /// </summary>
        /// <param name="commentData"></param>
        private void ShowUserSetting(object data)
        {
            if (data is CommentDataEx commentData)
            {

                var userDataManager = UserDataManager.Instance;
                var usersData = userDataManager.FirstOrDefault(x => x.LiveName.Equals(commentData.LiveName) && x.UserID.Equals(commentData.UserID));

                // ウィンドウ表示
                WindowManager.ShowUserSettingWindow(usersData ?? new(commentData));
            }
        }

        /// <summary>
        /// 項目右クリック時の子メニューを作成
        /// </summary>
        private void MenuItemCopyOpened(MenuItem menu)
        {
            var owner = menu.GetParentObject().FindAncestor<ContextMenu>();
            var commentData = (owner.PlacementTarget as ListViewItem).Content as CommentDataEx;

            if (commentData is not null)
            {
                menu.Items.Clear();

                // コンテキストメニュー設定
                Utility.CreateMenuItemToCopy(menu, commentData.LiveName);
                Utility.CreateMenuItemToCopy(menu, commentData.PostTime.ToString("HH:mm:ss"));
                Utility.CreateMenuItemToCopy(menu, commentData.UserID);
                Utility.CreateMenuItemToCopy(menu, commentData.UserName);
                Utility.CreateMenuItemToCopy(menu, commentData.Comment);

                // コメントデータからURL検出
                Utility.CreateMenuItemToURL(menu, commentData.Comment);
            }
        }
    }
}

using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MCC.Core;
using MCC.Core.Manager;
using MCC.Plugin.Win;
using MCC.Utility;
using MCC.Utility.IO;
using MultiCommentCollector.Helper;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Serialization;

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

        private Setting setting = Setting.GetInstance();

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
        public ReactiveCommand<CommentDataEx> ShowUserSettingCommand { get; }
        public ReactiveCollection<MenuItem> ParentMenuPlugins { get; }
        public ReactiveCommand<MenuItem> MenuItemOpenedCommand { get; }

        public MainWindowViewModel()
        {
            WindowManager.ApplicationStart();

            Width = setting.MainWindow.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Height = setting.MainWindow.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Left = setting.MainWindow.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Top = setting.MainWindow.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            IsPaneOpen = setting.IsPaneOpen.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            PaneWidth = setting.PaneWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            ShowLogWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowLogWindow).AddTo(disposable);
            ShowPluginWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowPluginWindow).AddTo(disposable);
            ShowOptionWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowOptionWindow).AddTo(disposable);
            ShowUsersSettingWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowUsersSettingWindow).AddTo(disposable);
            ApplicationShutdownCommand = new ReactiveCommand().WithSubscribe(WindowManager.ApplicationShutdown).AddTo(disposable);
            EnterCommand = new ReactiveCommand<string>().WithSubscribe(x => MCC.Core.Win.MultiCommentCollector.GetInstance().AddURL(x)).AddTo(disposable);
            ActivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => MCC.Core.Win.MultiCommentCollector.GetInstance().Activate(x)).AddTo(disposable);
            InactivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => MCC.Core.Win.MultiCommentCollector.GetInstance().Inactivate(x)).AddTo(disposable);
            DeleteCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x =>
            {
                MCC.Core.Win.MultiCommentCollector.GetInstance().Inactivate(x);
                PluginManager.GetInstance().Remove(x.Plugin);
                ConnectionManager.GetInstance().Remove(x);
            }).AddTo(disposable);
            ToggleCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x =>
            {
                if (x is null)
                    return;
                if (x.IsActive.Value)
                    MCC.Core.Win.MultiCommentCollector.GetInstance().Inactivate(x);
                else
                    MCC.Core.Win.MultiCommentCollector.GetInstance().Activate(x);

            }).AddTo(disposable);
            ColumnHeaderClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(x => UserHeaderClick(x)).AddTo(disposable);
            ShowUserSettingCommand = new ReactiveCommand<CommentDataEx>().WithSubscribe(x => ShowUserSetting(x)).AddTo(disposable);
            ParentMenuPlugins = new ReactiveCollection<MenuItem>().AddTo(disposable);

            AddMenuItem();

            // Theme
            setting.Theme.IsDarkMode.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(x ? "Dark" : "Light")}.{setting.Theme.ThemeColor.Value}"));
            setting.Theme.ThemeColor.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(setting.Theme.IsDarkMode.Value ? "Dark" : "Light")}.{x}"));

            disposable.Add(setting.Theme.IsDarkMode);
            disposable.Add(setting.Theme.ThemeColor);

            // ContextMenu
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(x => MenuItemCopyOpened(x)).AddTo(disposable);
        }

        private void UserHeaderClick(RoutedEventArgs e)
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
        private void MeunItemClick(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;

            if (item.Items is not null && item.Items.Count > 0)
            {
                var window = new MahApps.Metro.Controls.MetroWindow() { Owner = Application.Current.MainWindow };

                ((ISetting)item.Items[0]).ShowWindow(window);

                SetHeaderFontSize(window);
            }
        }

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

        private void AddMenuItem()
        {
            var pluginManager = PluginManager.GetInstance();
            var parent = pluginManager.Parent.Select(x => new MenuItem() { Header = x.PluginName });

            foreach (var item in parent)
            {
                item.ItemsSource = pluginManager.Parent.Where(x => x.PluginName.Equals(item.Header) && x is ISetting);
                item.DisplayMemberPath = "MenuItemName";
                item.Click += MeunItemClick;

                // 追加
                ParentMenuPlugins.Add(item);
            }
        }

        private void ShowUserSetting(CommentDataEx commentData)
        {
            if (commentData is null)
                return;

            var userDataManager = UserDataManager.GetInstance();
            var usersData = userDataManager.Where(x => x.LiveName.Equals(commentData.LiveName) && x.UserID.Equals(commentData.UserID)).ToArray();

            // ウィンドウ表示
            WindowManager.ShowUserDataWindow(usersData.Length > 0 ? usersData[0] : new(commentData));
        }

        private void MenuItemCopyOpened(MenuItem menu)
        {
            var owner = menu.GetParentObject().FindAncestor<ContextMenu>();
            var commentData = ((ListViewItem)owner.PlacementTarget).Content as CommentDataEx;

            if (commentData is not null)
            {
                menu.Items.Clear();

                var mainContent = new MenuItem();
                mainContent.Header = commentData.Comment;
                mainContent.Click += CopyItem_Click;

                // コンテキストメニュー設定
                menu.Items.Add(mainContent);

                var separator = false;
                var reg = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
                var r = new Regex(reg, RegexOptions.IgnoreCase);
                var collection = r.Matches(commentData.Comment);

                foreach (Match m in collection)
                {
                    if (m.Success)
                    {
                        if (!separator)
                        {
                            menu.Items.Add(new Separator());
                            separator = true;
                        }

                        var url = new MenuItem();
                        url.Header = m.Value;
                        url.Click += CopyItem_Click;

                        // コンテキストメニュー設定
                        menu.Items.Add(url);
                    }
                }
            }
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, ((MenuItem)sender).Header);
        }
    }
}

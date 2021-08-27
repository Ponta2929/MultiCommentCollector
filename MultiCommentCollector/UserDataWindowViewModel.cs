using MahApps.Metro.Controls;
using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Helper;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiCommentCollector
{
    internal class UserDataWindowViewModel : INotifyPropertyChanged, IDisposable
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

        private string liveName, userId;
        public ReactiveCommand<MenuItem> MenuItemOpenedCommand { get; }


        public CollectionViewSource Filterd { get; }

        public UserDataWindowViewModel(CommentDataEx user)
        {
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(MenuItemCopyOpened).AddTo(disposable);

            this.liveName = user.LiveName;
            this.userId = user.UserID;

            Filterd = new CollectionViewSource()
            {
                Source = CommentManager.Instance
            };
            Filterd.Filter += Filterd_Filter;
        }

        private void Filterd_Filter(object sender, FilterEventArgs e)
        {
            var item = e.Item as CommentDataEx;

            if (item is not null && liveName == item.LiveName && userId == item.UserID)
                e.Accepted = true;
            else
                e.Accepted = false;
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
                CreateMenuItemToCopy(menu, commentData.LiveName);
                CreateMenuItemToCopy(menu, commentData.PostTime.ToString("HH:mm:ss"));
                CreateMenuItemToCopy(menu, commentData.UserID);
                CreateMenuItemToCopy(menu, commentData.UserName);
                CreateMenuItemToCopy(menu, commentData.Comment);

                // コメントデータからURL検出
                CreateMenuItemToURL(menu, commentData.Comment);
            }
        }

        /// <summary>
        /// コピー用メニューを作成
        /// </summary>
        private bool CreateMenuItemToCopy(MenuItem owner, string header)
        {
            if (header == null || header.Equals(""))
                return false;

            var content = new MenuItem();
            content.Header = header;
            content.Click += MenuItemCopy_Click;

            owner.Items.Add(content);

            return true;
        }

        /// <summary>
        /// コピー用メニューを作成(URL)
        /// </summary>
        private bool CreateMenuItemToURL(MenuItem owner, string header)
        {
            var separator = false;
            var reg = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            var r = new Regex(reg, RegexOptions.IgnoreCase);
            var collection = r.Matches(header);

            foreach (Match m in collection)
            {
                if (m.Success)
                {
                    if (!separator)
                    {
                        owner.Items.Add(new Separator());
                        separator = true;
                    }

                    // コンテキストメニュー設定
                    CreateMenuItemToCopy(owner, m.Value);
                }
            }

            return separator;
        }

        private void MenuItemCopy_Click(object sender, RoutedEventArgs _)
            => Clipboard.SetData(DataFormats.Text, (sender as MenuItem).Header);
    }
}

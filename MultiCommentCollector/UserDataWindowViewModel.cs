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

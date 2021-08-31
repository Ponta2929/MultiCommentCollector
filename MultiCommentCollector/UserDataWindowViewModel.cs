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
using System.Windows.Media;

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
        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<SolidColorBrush> BackColor { get; }
        public ReactiveCommand<MenuItem> MenuItemOpenedCommand { get; }

        public CollectionViewSource CommentFilter { get; }

        public UserDataWindowViewModel(CommentDataEx user)
        {
            Title = new ReactiveProperty<string>().AddTo(disposable);
            BackColor = new ReactiveProperty<SolidColorBrush>(new SolidColorBrush( Color.FromArgb(255, 0, 0, 0))).AddTo(disposable);
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(MenuItemCopyOpened).AddTo(disposable);

            user.ObserveProperty(o => o.UserName).Subscribe(value => Title.Value = $"{user.LiveName} - " + (user.UserName is null || user.UserName.Equals("") ? user.UserID : user.UserName)).AddTo(disposable);
            user.ObserveProperty(o => o.BackColor).Subscribe(value => BackColor.Value.Color = Color.FromArgb((byte)value.A, (byte)value.R, (byte)value.G, (byte)value.B)).AddTo(disposable);

            this.liveName = user.LiveName;
            this.userId = user.UserID;

            CommentFilter = new CollectionViewSource()
            {
                Source = CommentManager.Instance
            };
            CommentFilter.Filter += CommentFilter_Filter;
        }

        private void CommentFilter_Filter(object sender, FilterEventArgs e)
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
        private void MenuItemCopyOpened(MenuItem menuItem)
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

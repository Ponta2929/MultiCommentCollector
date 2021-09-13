using MahApps.Metro.Controls;
using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiCommentCollector.ViewModels
{
    internal class UserDataWindowViewModel : ViewModelBase
    {
        public string LiveName { get; private set; }
        public string UserId { get; private set; }

        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<SolidColorBrush> BackColor { get; }
        public ReactiveCommand ShowUserSettingCommand { get; }
        public ReactiveCommand<MenuItem> MenuItemOpenedCommand { get; }

        public CollectionViewSource CommentFilter { get; }

        public UserDataWindowViewModel(CommentDataEx user)
        {
            this.LiveName = user.LiveName;
            this.UserId = user.UserID;

            Title = new ReactiveProperty<string>().AddTo(Disposable);
            BackColor = new ReactiveProperty<SolidColorBrush>(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))).AddTo(Disposable);
            ShowUserSettingCommand = new ReactiveCommand().WithSubscribe(() => WindowManager.ShowUserSettingWindow(user)).AddTo(Disposable);
            MenuItemOpenedCommand = new ReactiveCommand<MenuItem>().WithSubscribe(MenuItemCopyOpened).AddTo(Disposable);

            CommentFilter = new() { Source = CommentManager.Instance };
            CommentFilter.Filter += CommentFilter_Filter;

            user.ObserveProperty(o => o.UserName).Subscribe(value => Title.Value = $"{user.LiveName} - " + (user.UserName is null || user.UserName.Equals("") ? user.UserID : user.UserName)).AddTo(Disposable);
            user.ObserveProperty(o => o.BackColor).Subscribe(value => BackColor.Value.Color = value.ToArgb()).AddTo(Disposable);
        }

        private void CommentFilter_Filter(object sender, FilterEventArgs e)
        {
            var item = e.Item as CommentDataEx;

            if (item is not null && LiveName == item.LiveName && UserId == item.UserID)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
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

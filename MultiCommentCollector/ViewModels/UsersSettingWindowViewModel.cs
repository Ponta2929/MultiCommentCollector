using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModels
{
    internal class UsersSettingWindowViewModel : ViewModelBase
    {
        private UserDataManager userDataManager = UserDataManager.Instance;
        private CommentManager commentManager = CommentManager.Instance;

        public ReactiveProperty<string> SearchText { get; }
        public ReactiveCommand RefleshCommand { get; }
        public ReactiveCommand<RoutedEventArgs> ColumnHeaderClickCommand { get; }
        public ReactiveCommand<UserData> ShowUserSettingCommand { get; }
        public ReactiveCommand<UserData> DeleteUserSettingCommand { get; }
        public CollectionViewSource UsersDataView { get; }

        public UsersSettingWindowViewModel()
        {
            SearchText = new ReactiveProperty<string>("").AddTo(Disposable);
            RefleshCommand = new ReactiveCommand().WithSubscribe(() => MessageBroker.Default.Publish<MessageArgs>(new() { Identifier = "Refresh.Comment.View" })).AddTo(Disposable);
            ColumnHeaderClickCommand = new ReactiveCommand<RoutedEventArgs>().WithSubscribe(ColumnHeader_Click).AddTo(Disposable);
            ShowUserSettingCommand = new ReactiveCommand<UserData>().WithSubscribe(WindowManager.ShowUserSettingWindow).AddTo(Disposable);
            DeleteUserSettingCommand = new ReactiveCommand<UserData>().WithSubscribe(DeleteUserSetting).AddTo(Disposable);

            UsersDataView = new() { Source = userDataManager };
            UsersDataView.Filter += UsersDataView_Filter;

            SearchText.Subscribe(x => UsersDataView.View.Refresh()).AddTo(Disposable);
        }

        private void UsersDataView_Filter(object _, FilterEventArgs e)
        {
            var item = e.Item as UserData;
            var word = SearchText.Value.ToLower();

            if (SearchText.Value is null || SearchText.Value is "" || item.LiveName.ToLower().Contains(word) || item.UserID.ToLower().Contains(word) || item.UserName.ToLower().Contains(word))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void DeleteUserSetting(UserData user)
        {
            if (user is null)
            {
                return;
            }

            // 削除
            if (userDataManager.Remove(commentManager, user))
            {
                MessageBroker.Default.Publish<MessageArgs>(new() { Identifier = "Refresh.Comment.View" });

                // ユーザー設定保存
                SerializerHelper.XmlSerialize("users.xml", userDataManager);
            }
        }

        private void ColumnHeader_Click(RoutedEventArgs e)
        {
            var owner = (e.OriginalSource as DependencyObject).FindAncestor<ListView>();
            var header = (e.OriginalSource as DependencyObject).FindAncestor<GridViewColumnHeader>();

            if (header is not null)
            {
                if (header.Role is not GridViewColumnHeaderRole.Padding)
                {
                    if (header != owner.Tag)
                    {
                        header.Tag = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (header.Tag is ListSortDirection.Ascending)
                        {
                            header.Tag = ListSortDirection.Descending;
                        }
                        else
                        {
                            header.Tag = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = header.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? header.Column.Header as string;

                    if (sortBy.Equals("非表示"))
                    {
                        sortBy = "HideUser";
                    }
                    else if (sortBy.Equals("背景色"))
                    {
                        sortBy = "BackColor";
                    }

                    Sort(sortBy, (ListSortDirection)header.Tag);

                    owner.Tag = header;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            UsersDataView.View.SortDescriptions.Clear();
            UsersDataView.View.SortDescriptions.Add(new(sortBy, direction));
            UsersDataView.View.Refresh();
        }
    }
}

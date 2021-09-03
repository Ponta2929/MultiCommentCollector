using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Model;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModel
{
    internal class UsersSettingWindowViewModel : INotifyPropertyChanged, IDisposable
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

        private UserDataManager userDataManager = UserDataManager.Instance;
        private CommentManager commentManager = CommentManager.Instance;
        private UserSetting userSetting = UserSetting.Instance;

        public ReactiveCommand RefleshCommand { get; }
        public ReactiveCommand<UserData> ShowUserSettingCommand { get; }
        public ReactiveCommand<UserData> DeleteUserSettingCommand { get; }
        public CollectionViewSource UsersDataView { get; }

        public UsersSettingWindowViewModel()
        {
            UsersDataView = new() { Source = userDataManager };

            RefleshCommand = new ReactiveCommand().WithSubscribe(() => MessageBroker.Default.Publish<string>("Refresh.Comment.View")).AddTo(disposable);
            ShowUserSettingCommand = new ReactiveCommand<UserData>().WithSubscribe(WindowManager.ShowUserSettingWindow).AddTo(disposable);
            DeleteUserSettingCommand = new ReactiveCommand<UserData>().WithSubscribe(DeleteUserSetting).AddTo(disposable);
        }

        private void DeleteUserSetting(UserData user)
        {
            if (user is null)
                return;

            // 削除
            if (userDataManager.Remove(commentManager, user))
            {
                MessageBroker.Default.Publish<string>("Refresh.Comment.View");

                // ユーザー設定保存
                userSetting.UserDataList = userDataManager;

                SerializeHelper.SaveToXml<UserSetting>("users.xml", userSetting);
            }
        }
    }
}

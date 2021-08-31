using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollector
{
    class UsersSettingWindowViewModel : INotifyPropertyChanged, IDisposable
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

        public UsersSettingWindowViewModel()
        {
            RefleshCommand = new ReactiveCommand().WithSubscribe(() => MessageBroker.Default.Publish<UserData>(null)).AddTo(disposable);
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
                MessageBroker.Default.Publish<UserData>(null);

                // ユーザー設定保存
                userSetting.UserDataList = userDataManager;

                SerializeHelper.SaveToXml<UserSetting>("users.xml", userSetting);
            }
        }
    }
}

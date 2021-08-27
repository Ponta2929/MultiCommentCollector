using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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

        public ReactiveCommand<UserData> ShowUserSettingCommand { get; }
        public ReactiveCommand<UserData> DeleteUserSettingCommand { get; }

        public UsersSettingWindowViewModel()
        {
            ShowUserSettingCommand = new ReactiveCommand<UserData>().WithSubscribe(ShowUserSetting).AddTo(disposable);
            DeleteUserSettingCommand = new ReactiveCommand<UserData>().WithSubscribe(DeleteUserSetting).AddTo(disposable);
        }

        private void ShowUserSetting(UserData userData)
        {
            if (userData is null)
                return;

            // ウィンドウ表示
            WindowManager.ShowUserSettingWindow(userData);
        }
        private void DeleteUserSetting(UserData user)
        {
            if (user is null)
                return;

            var userDataManager = UserDataManager.Instance;

            // 削除
            if (userDataManager.Remove(CommentManager.Instance, user))
            {
                // ユーザー設定保存
                var userSetting = UserSetting.Instance;
                userSetting.UserDataList = userDataManager;

                Utility.SaveToXml<UserSetting>("users.xml", userSetting);
            }
        }
    }
}

using MCC.Core.Manager;
using MCC.Utility;
using MCC.Utility.IO;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MultiCommentCollector
{
    internal class UserSettingWindowViewModel : INotifyPropertyChanged, IDisposable
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

        public ReactiveProperty<string> LiveName { get; }
        public ReactiveProperty<string> UserID { get; }
        public ReactiveProperty<string> UserName { get; }
        public ReactiveProperty<Color> BackgroundColor { get; }
        public ReactiveCommand CloseWindowCommand { get; }
        public ReactiveCommand OkClickCommand { get; }

        public UserSettingWindowViewModel(UserData user)
        {
            LiveName = new ReactiveProperty<string>(user.LiveName).AddTo(disposable);
            UserID = new ReactiveProperty<string>(user.UserID).AddTo(disposable);
            UserName = new ReactiveProperty<string>(user.UserName).AddTo(disposable);
            BackgroundColor = new ReactiveProperty<Color>(Color.FromArgb((byte)user.BackColor.A, (byte)user.BackColor.R, (byte)user.BackColor.G, (byte)user.BackColor.B)).AddTo(disposable);

            // Commands
            OkClickCommand = new ReactiveCommand().WithSubscribe(OkButtonClick).AddTo(disposable);
            CloseWindowCommand = new ReactiveCommand().WithSubscribe(() => WindowManager.CloseWindow(this)).AddTo(disposable);
        }

        private void OkButtonClick()
        {
            var userDataManager = UserDataManager.Instance;
            var commentManager = CommentManager.Instance;
            var user = new UserData()
            {
                UserID = UserID.Value,
                UserName = UserName.Value,
                LiveName = LiveName.Value,
                BackColor = ColorData.FromArgb(BackgroundColor.Value.A, BackgroundColor.Value.R, BackgroundColor.Value.G, BackgroundColor.Value.B)
            };

            // コメント更新
            userDataManager.Update(user);
            commentManager.Update(user);

            // ユーザー設定保存
            var userSetting = UserSetting.Instance;
            userSetting.UserDataList = userDataManager;

            SerializeHelper.SaveToXml<UserSetting>("users.xml", userSetting);

            WindowManager.CloseWindow(this);
        }
    }
}

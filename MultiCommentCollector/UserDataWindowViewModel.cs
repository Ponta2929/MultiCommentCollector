using MCC.Core.Manager;
using MCC.Utility;
using MCC.Utility.IO;
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

        public ReactiveProperty<string> LiveName { get; }
        public ReactiveProperty<string> UserID { get; }
        public ReactiveProperty<string> UserName { get; }
        public ReactiveProperty<Color> BackgroundColor { get; }
        public ReactiveCommand CloseWindowCommand { get; }
        public ReactiveCommand OkClickCommand { get; }

        public UserDataWindowViewModel()
        {
            LiveName = new ReactiveProperty<string>("").AddTo(disposable);
            UserID = new ReactiveProperty<string>("").AddTo(disposable);
            UserName = new ReactiveProperty<string>("").AddTo(disposable);
            BackgroundColor = new ReactiveProperty<Color>(Color.FromArgb(0, 0, 0, 0)).AddTo(disposable);

            // 購読
            OkClickCommand = new ReactiveCommand().WithSubscribe(OkButtonClick).AddTo(disposable);
            CloseWindowCommand = new ReactiveCommand().WithSubscribe(() => WindowManager.CloseWindow(this)).AddTo(disposable);

            MessageBroker.Default.Subscribe<UserData>(SetUserData).AddTo(disposable);
        }

        private void OkButtonClick()
        {
            var userDataManager = UserDataManager.GetInstance();
            var userData = userDataManager.Where(x => x.LiveName.Equals(LiveName.Value) && x.UserID.Equals(UserID.Value)).ToArray();

            if (userData.Length > 0)
            {
                userData[0].UserName = UserName.Value;
                userData[0].BackColor = ColorData.FromArgb(BackgroundColor.Value.A, BackgroundColor.Value.R, BackgroundColor.Value.G, BackgroundColor.Value.B);
            }
            else
            {
                userDataManager.Add(new()
                {
                    UserID = UserID.Value,
                    UserName = UserName.Value,
                    LiveName = LiveName.Value,
                    BackColor = ColorData.FromArgb(BackgroundColor.Value.A, BackgroundColor.Value.R, BackgroundColor.Value.G, BackgroundColor.Value.B)
                });
            }
            // コメント更新
            CommentDataRefresh();

            // ユーザー設定保存
            var userSetting = UserSetting.GetInstance();
            userSetting.UserDataList = userDataManager;
            Utility.SaveToXml<UserSetting>("users.xml", userSetting);

            WindowManager.CloseWindow(this);
        }

        /// <summary>
        /// ユーザーデータ設定
        /// </summary>
        private void SetUserData(UserData user)
        {
            LiveName.Value = user.LiveName;
            UserID.Value = user.UserID;
            UserName.Value = user.UserName;
            BackgroundColor.Value = Color.FromArgb((byte)user.BackColor.A, (byte)user.BackColor.R, (byte)user.BackColor.G, (byte)user.BackColor.B);
        }

        private void CommentDataRefresh()
        {
            var userDataManager = UserDataManager.GetInstance();

            foreach (var item in CommentManager.GetInstance())
            {
                var userData = userDataManager.Where(x => x.LiveName.Equals(item.LiveName) && x.UserID.Equals(item.UserID)).ToArray();

                if (userData.Length > 0)
                {
                    item.BackColor = userData[0].BackColor;
                    item.UserName = userData[0].UserName;
                }
            }
        }
    }
}

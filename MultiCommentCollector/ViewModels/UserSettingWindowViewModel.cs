using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System.Windows.Media;

namespace MultiCommentCollector.ViewModels
{
    internal class UserSettingWindowViewModel : ViewModelBase
    {
        public ReactiveProperty<bool> HideUser { get; }
        public ReactiveProperty<string> LiveName { get; }
        public ReactiveProperty<string> UserID { get; }
        public ReactiveProperty<string> UserName { get; }
        public ReactiveProperty<Color> BackgroundColor { get; }
        public ReactiveCommand CloseWindowCommand { get; }
        public ReactiveCommand OkClickCommand { get; }

        public UserSettingWindowViewModel(UserData user)
        {
            HideUser = new ReactiveProperty<bool>(user.HideUser).AddTo(Disposable);
            LiveName = new ReactiveProperty<string>(user.LiveName).AddTo(Disposable);
            UserID = new ReactiveProperty<string>(user.UserID).AddTo(Disposable);
            UserName = new ReactiveProperty<string>(user.UserName).AddTo(Disposable);
            BackgroundColor = new ReactiveProperty<Color>(Color.FromArgb((byte)user.BackColor.A, (byte)user.BackColor.R, (byte)user.BackColor.G, (byte)user.BackColor.B)).AddTo(Disposable);

            // Commands
            OkClickCommand = new ReactiveCommand().WithSubscribe(OkButtonClick).AddTo(Disposable);
            CloseWindowCommand = new ReactiveCommand().WithSubscribe(() => WindowHelper.CloseWindow(this)).AddTo(Disposable);
        }

        private void OkButtonClick()
        {
            var user = new UserData()
            {
                HideUser = HideUser.Value,
                UserID = UserID.Value,
                UserName = UserName.Value,
                LiveName = LiveName.Value,
                BackColor = ColorDataExtensions.FromColor(BackgroundColor.Value)
            };

            // コメント更新
            UserDataManager.Instance.Update(user);
            CommentManager.Instance.Update(user);

            // ユーザー設定保存
            SerializerHelper.XmlSerialize("users.xml", UserDataManager.Instance);
            MessageBroker.Default.Publish<MessageArgs>(new() { Identifier = "Refresh.Comment.View" });

            WindowHelper.CloseWindow(this);
        }
    }
}

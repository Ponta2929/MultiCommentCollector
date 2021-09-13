using MultiCommentCollector.Extensions;
using MultiCommentCollector.Helper;
using MultiCommentCollector.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Windows.Media;

namespace MultiCommentCollector.ViewModels
{
    internal class OptionWindowViewModel : ViewModelBase
    {
        private ArrayLimit limits = Setting.Instance.ArrayLimits;
        private Server servers = Setting.Instance.Servers;
        private Models.Theme theme = Setting.Instance.Theme;

        public ReactiveProperty<int> CommentReceiverServerPort { get; init; }
        public ReactiveProperty<int> CommentGeneratorServerPort { get; init; }
        public ReactiveProperty<int> MaxComments { get; }
        public ReactiveProperty<int> MaxLogs { get; }
        public ReactiveProperty<bool> IsDarkMode { get; }
        public ReactiveProperty<Color> ThemeColor { get; }

        public OptionWindowViewModel()
        {
            MaxComments = limits.MaxComments.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            MaxLogs = limits.MaxLogs.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            CommentReceiverServerPort = servers.CommentReceiverServerPort.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            CommentGeneratorServerPort = servers.CommentGeneratorServerPort.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            IsDarkMode = theme.IsDarkMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            ThemeColor = theme.ThemeColor.ToReactivePropertyAsSynchronized(x => x.Value, x => x.ToArgb(), x => ColorDataExtensions.FromColor(x)).AddTo(Disposable);

            IsDarkMode.Subscribe(x => ThemeHelper.ThemeChange(ThemeColor.Value, x)).AddTo(Disposable);
            ThemeColor.Subscribe(x => ThemeHelper.ThemeChange(x, IsDarkMode.Value)).AddTo(Disposable);
        }
    }
}
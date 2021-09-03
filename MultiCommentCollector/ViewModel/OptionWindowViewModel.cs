using ControlzEx.Theming;
using MCC.Core.Manager;
using MultiCommentCollector.Model;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows;

namespace MultiCommentCollector.ViewModel
{
    internal class OptionWindowViewModel : INotifyPropertyChanged, IDisposable
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

        private Setting setting = Setting.Instance;

        public ReactiveProperty<int> CommentReceiverServerPort { get; init; }
        public ReactiveProperty<int> CommentGeneratorServerPort { get; init; }
        public ReactiveProperty<int> MaxComments { get; }
        public ReactiveProperty<int> MaxLogs { get; }
        public ReactiveProperty<bool> IsDarkMode { get; }
        public ReactiveProperty<string> ThemeColor { get; }
        public ReactiveCollection<string> Colors { get; }

        public OptionWindowViewModel()
        {
            MaxComments = setting.MaxComments.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            MaxLogs = setting.MaxLogs.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            CommentReceiverServerPort = setting.Servers.CommentReceiverServerPort.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            CommentGeneratorServerPort = setting.Servers.CommentGeneratorServerPort.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            IsDarkMode = setting.Theme.IsDarkMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            ThemeColor = setting.Theme.ThemeColor.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            IsDarkMode.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(x ? "Dark" : "Light")}.{ThemeColor.Value}")).AddTo(disposable);
            ThemeColor.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(IsDarkMode.Value ? "Dark" : "Light")}.{x}")).AddTo(disposable);
            MaxComments.Subscribe(x => CommentManager.Instance.MaxSize.Value = x);
            MaxLogs.Subscribe(x => LogManager.Instance.MaxSize.Value = x);

            Colors = new ReactiveCollection<string>().AddTo(disposable);
            Colors.AddRangeOnScheduler(Setting.Instance.Theme.Colors);
        }
    }
}
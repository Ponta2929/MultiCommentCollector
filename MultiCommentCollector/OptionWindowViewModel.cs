using ControlzEx.Theming;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MultiCommentCollector
{
    public class OptionWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CompositeDisposable disposable = new();

        public void Dispose()
        {
            disposable.Clear();
            disposable.Dispose();
        }

        private Setting setting = Setting.GetInstance();

        public ReactiveProperty<int> CommentReceiverServerPort { get; init; }
        public ReactiveProperty<int> CommentGeneratorServerPort { get; init; }
        public ReactiveProperty<int> MaxComments { get; }
        public ReactiveProperty<int> MaxLogs { get; }
        public ReactiveProperty<bool> IsDarkMode { get; }
        public ReactiveProperty<string> ThemeColor { get; }

        public OptionWindowViewModel()
        {
            MaxComments = setting.MaxComments.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            MaxLogs = setting.MaxLogs.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            CommentReceiverServerPort = setting.Servers.CommentReceiverServerPort.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            CommentGeneratorServerPort = setting.Servers.CommentGeneratorServerPort.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            IsDarkMode = setting.Theme.IsDarkMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            ThemeColor = setting.Theme.ThemeColor.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            IsDarkMode.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(x ? "Dark" : "Light")}.{ThemeColor.Value}"));
            ThemeColor.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(IsDarkMode.Value ? "Dark" : "Light")}.{x}"));

            disposable.Add(IsDarkMode);
            disposable.Add(ThemeColor);
        }
    }
}
;
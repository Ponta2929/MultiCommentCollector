using ControlzEx.Theming;
using MCC.Core;
using MCC.Utility;
using MCC.Utility.IO;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MultiCommentCollector
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CompositeDisposable disposable = new();

        public void Dispose()
        {
            disposable.Clear();
            disposable.Dispose();
        }

        private Setting setting = Setting.GetInstance();

        public ReactiveProperty<double> Width { get; }
        public ReactiveProperty<double> Height { get; }
        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<bool> IsPaneOpen { get; }
        public ReactiveProperty<int> PaneWidth { get; }

        public ReactiveCommand ShowLogWindowCommand { get; }
        public ReactiveCommand ShowOptionWindowCommand { get; }
        public ReactiveCommand ApplicationShutdownCommand { get; }
        public ReactiveCommand<string> EnterCommand { get; }
        public ReactiveCommand<ConnectionData> ActivateCommand { get; }
        public ReactiveCommand<ConnectionData> InactivateCommand { get; }
        public ReactiveCommand<ConnectionData> DeleteCommand { get; }
        public ReactiveCommand<ConnectionData> ToggleCommand { get; }

        public MainWindowViewModel()
        {
            WindowManager.ApplicationStart();

            Width = setting.MainWindow.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Height = setting.MainWindow.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Left = setting.MainWindow.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Top = setting.MainWindow.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            IsPaneOpen = setting.IsPaneOpen.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            PaneWidth = setting.PaneWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            ShowLogWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowLogWindow).AddTo(disposable);
            ShowOptionWindowCommand = new ReactiveCommand().WithSubscribe(WindowManager.ShowOptionWindow).AddTo(disposable);
            ApplicationShutdownCommand = new ReactiveCommand().WithSubscribe(WindowManager.ApplicationShutdown).AddTo(disposable);
            EnterCommand = new ReactiveCommand<string>().WithSubscribe(x => MCC.Core.MultiCommentCollector.GetInstance().AddURL(x)).AddTo(disposable);
            ActivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => MCC.Core.MultiCommentCollector.GetInstance().Activate(x)).AddTo(disposable);
            InactivateCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x => MCC.Core.MultiCommentCollector.GetInstance().Inactivate(x)).AddTo(disposable);
            DeleteCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x =>
            {
                MCC.Core.MultiCommentCollector.GetInstance().Inactivate(x);
                ConnectionManager.GetInstance().Items.Remove(x);
            }).AddTo(disposable);
            ToggleCommand = new ReactiveCommand<ConnectionData>().WithSubscribe(x =>
            {
                if (x is null)
                    return;
                if (x.IsActive.Value)
                    MCC.Core.MultiCommentCollector.GetInstance().Inactivate(x);
                else
                    MCC.Core.MultiCommentCollector.GetInstance().Activate(x);
            }).AddTo(disposable);

            setting.Theme.IsDarkMode.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(x ? "Dark" : "Light")}.{setting.Theme.ThemeColor.Value}"));
            setting.Theme.ThemeColor.Subscribe(x => ThemeManager.Current.ChangeTheme(Application.Current, $"{(setting.Theme.IsDarkMode.Value ? "Dark" : "Light")}.{x}"));

            disposable.Add(setting.Theme.IsDarkMode);
            disposable.Add(setting.Theme.ThemeColor);
        }
    }
}

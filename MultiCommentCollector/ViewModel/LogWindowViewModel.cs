using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Model;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModel
{
    internal class LogWindowViewModel : INotifyPropertyChanged, IDisposable
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

        public ReactiveProperty<double> Width { get; }
        public ReactiveProperty<double> Height { get; }
        public ReactiveProperty<double> Left { get; }
        public ReactiveProperty<double> Top { get; }
        public ReactiveProperty<bool> ContextMenuError { get; }
        public ReactiveProperty<bool> ContextMenuWarn { get; }
        public ReactiveProperty<bool> ContextMenuInfo { get; }
        public ReactiveProperty<bool> ContextMenuDebug { get; }
        public CollectionViewSource LogFilterView { get; }

        public LogWindowViewModel()
        {
            // フィルター            
            LogFilterView = new() { Source = LogManager.Instance };
            LogFilterView.Filter += LogFilterViewr_Filter;

            Width = setting.LogWindow.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Height = setting.LogWindow.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Left = setting.LogWindow.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Top = setting.LogWindow.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            ContextMenuError = new ReactiveProperty<bool>().AddTo(disposable);
            ContextMenuWarn = new ReactiveProperty<bool>().AddTo(disposable);
            ContextMenuInfo = new ReactiveProperty<bool>(true).AddTo(disposable);
            ContextMenuDebug = new ReactiveProperty<bool>().AddTo(disposable);

            ContextMenuError.Subscribe(x => LogFilterView.View.Refresh()).AddTo(disposable);
            ContextMenuWarn.Subscribe(x => LogFilterView.View.Refresh()).AddTo(disposable);
            ContextMenuInfo.Subscribe(x => LogFilterView.View.Refresh()).AddTo(disposable);
            ContextMenuDebug.Subscribe(x => LogFilterView.View.Refresh()).AddTo(disposable);
        }

        private void LogFilterViewr_Filter(object sender, FilterEventArgs e)
        {
            var item = e.Item as LogData;

            if (item.Level == LogLevel.Debug && ContextMenuDebug.Value)
                e.Accepted = true;
            else if (item.Level == LogLevel.Info && ContextMenuInfo.Value)
                e.Accepted = true;
            else if (item.Level == LogLevel.Warn && ContextMenuWarn.Value)
                e.Accepted = true;
            else if (item.Level == LogLevel.Error && ContextMenuError.Value)
                e.Accepted = true;
            else
                e.Accepted = false;
        }
    }
}

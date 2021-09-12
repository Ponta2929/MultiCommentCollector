using MCC.Core.Manager;
using MCC.Utility;
using MultiCommentCollector.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows.Data;

namespace MultiCommentCollector.ViewModels
{
    internal class LogWindowViewModel : ViewModelBase
    {
        private WindowRect window = Setting.Instance.LogWindow;
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
            Width = window.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            Height = window.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            Left = window.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);
            Top = window.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(Disposable);

            ContextMenuError = new ReactiveProperty<bool>().AddTo(Disposable);
            ContextMenuWarn = new ReactiveProperty<bool>().AddTo(Disposable);
            ContextMenuInfo = new ReactiveProperty<bool>(true).AddTo(Disposable);
            ContextMenuDebug = new ReactiveProperty<bool>().AddTo(Disposable);

            // フィルター            
            LogFilterView = new() { Source = LogManager.Instance };
            LogFilterView.Filter += LogFilterView_Filter;

            ContextMenuError.Subscribe(x => LogFilterView.View.Refresh()).AddTo(Disposable);
            ContextMenuWarn.Subscribe(x => LogFilterView.View.Refresh()).AddTo(Disposable);
            ContextMenuInfo.Subscribe(x => LogFilterView.View.Refresh()).AddTo(Disposable);
            ContextMenuDebug.Subscribe(x => LogFilterView.View.Refresh()).AddTo(Disposable);
        }

        private void LogFilterView_Filter(object _, FilterEventArgs e)
        {
            var item = e.Item as LogData;

            if (item.Level is LogLevel.Debug && ContextMenuDebug.Value)
                e.Accepted = true;
            else if (item.Level is LogLevel.Info && ContextMenuInfo.Value)
                e.Accepted = true;
            else if (item.Level is LogLevel.Warn && ContextMenuWarn.Value)
                e.Accepted = true;
            else if (item.Level is LogLevel.Error && ContextMenuError.Value)
                e.Accepted = true;
            else
                e.Accepted = false;
        }
    }
}

using MCC.Core.Manager;
using MCC.Utility;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiCommentCollector
{
    public class LogWindowViewModel : INotifyPropertyChanged, IDisposable
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

        public LogWindowViewModel()
        {
            Width = setting.LogWindow.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Height = setting.LogWindow.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Left = setting.LogWindow.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Top = setting.LogWindow.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);

            ContextMenuError = new ReactiveProperty<bool>().AddTo(disposable);
            ContextMenuWarn = new ReactiveProperty<bool>().AddTo(disposable);
            ContextMenuInfo = new ReactiveProperty<bool>(true).AddTo(disposable);
            ContextMenuDebug = new ReactiveProperty<bool>().AddTo(disposable);

            ContextMenuError.Subscribe(x => Filtering()).AddTo(disposable);
            ContextMenuWarn.Subscribe(x => Filtering()).AddTo(disposable);
            ContextMenuInfo.Subscribe(x => Filtering()).AddTo(disposable);
            ContextMenuDebug.Subscribe(x => Filtering()).AddTo(disposable);

            CreateFilter();
        }

        private void CreateFilter()
        {
            var view = CollectionViewSource.GetDefaultView(LogManager.Instance);
            view.Filter = new Predicate<object>(FilterPredicate);

            var liveShaping = view as ICollectionViewLiveShaping;

            if (liveShaping.CanChangeLiveFiltering)
            {
                liveShaping.LiveFilteringProperties.Add("Level");
                liveShaping.IsLiveFiltering = true;
            }
        }

        private bool FilterPredicate(object @object)
        {
            var item = @object as LogData;

            if (item.Level == LogLevel.Debug && ContextMenuDebug.Value)
                return true;
            if (item.Level == LogLevel.Info && ContextMenuInfo.Value)
                return true;
            if (item.Level == LogLevel.Warn && ContextMenuWarn.Value)
                return true;
            if (item.Level == LogLevel.Error && ContextMenuError.Value)
                return true;

            return false;
        }

        private void Filtering()
            => CollectionViewSource.GetDefaultView(LogManager.Instance).Refresh();
    }
}

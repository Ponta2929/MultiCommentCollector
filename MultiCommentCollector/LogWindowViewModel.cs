using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentCollector
{
    public class LogWindowViewModel : INotifyPropertyChanged, IDisposable
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

        public LogWindowViewModel()
        {
            Width = setting.LogWindow.Width.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Height = setting.LogWindow.Height.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Left = setting.LogWindow.Left.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
            Top = setting.LogWindow.Top.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(disposable);
        }
    }
}

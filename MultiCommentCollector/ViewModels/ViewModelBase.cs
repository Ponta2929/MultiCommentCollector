using System;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace MultiCommentCollector.ViewModels
{
    internal class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        protected readonly CompositeDisposable Disposable = new();

        public void Dispose()
        {
            Disposable.Clear();
            Disposable.Dispose();
        }
    }
}

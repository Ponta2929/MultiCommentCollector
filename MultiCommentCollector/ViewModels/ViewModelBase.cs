using MCC.Utility.Binding;
using System;
using System.Reactive.Disposables;

namespace MultiCommentCollector.ViewModels
{
    internal class ViewModelBase : BindableBase, IDisposable
    {
        protected readonly CompositeDisposable Disposable = new();

        public void Dispose()
        {
            Disposable.Clear();
            Disposable.Dispose();
        }
    }
}

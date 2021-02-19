using Reactive.Bindings;
using System.Collections.Specialized;

namespace MCC.Core
{
    public class ListManagerBase<T> : ReactiveCollection<T>
    {
        private object syncObject = new();

        /// <summary>
        /// リストが保持できる件数です。
        /// </summary>
        public ReactivePropertySlim<int> MaxSize { get; set; } = new(1000);

        /// <summary>
        /// MaxSizeを有効化します。
        /// </summary>
        public ReactivePropertySlim<bool> IsLimit { get; set; } = new(true);

        public ListManagerBase()
        {
            CollectionChanged += OnCollectionChanged;
        }

        public new void Add(T item)
        {
            lock (syncObject)
            {
                AddOnScheduler(item);
            }
        }

        public new void RemoveAt(int index)
        {
            lock (syncObject)
            {
                RemoveAtOnScheduler(index);
            }
        }

        public new void Remove(T item)
        {
            lock (syncObject)
            {
                RemoveOnScheduler(item);
            }
        }

        public new void Clear()
        {
            lock (syncObject)
            {
                ClearOnScheduler();
            }
        }
        public void AddRange(params T[] items)
        {
            lock (syncObject)
            {
                AddRangeOnScheduler(items);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsLimit.Value)
                return;

            try
            {
                CollectionChanged -= OnCollectionChanged;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (Items.Count > MaxSize.Value)
                    {
                        RemoveAt(0);
                    }
                }
            }
            finally
            {
                CollectionChanged += OnCollectionChanged;
            }
        }
    }
}

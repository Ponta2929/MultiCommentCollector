using Reactive.Bindings;
using System.Collections.Specialized;

namespace MCC.Core.Manager
{
    public class ListManagerBase<T> : ReactiveCollection<T>
    {
        private object syncObject = new();

        /// <summary>
        /// リストが保持できる件数です。
        /// </summary>
        public ReactiveProperty<int> MaxSize { get; set; } = new(1000);

        /// <summary>
        /// MaxSizeを有効化します。
        /// </summary>
        public ReactiveProperty<bool> IsLimit { get; set; } = new(true);

        public ListManagerBase() => CollectionChanged += OnCollectionChanged;

        public void SyncAdd(T item)
        {
            lock (syncObject)
            {
                AddOnScheduler(item);
            }
        }

        public void SyncRemoveAt(int index)
        {
            lock (syncObject)
            {
                RemoveAtOnScheduler(index);
            }
        }

        public void SyncRemove(T item)
        {
            lock (syncObject)
            {
                RemoveOnScheduler(item);
            }
        }

        public void SyncClear()
        {
            lock (syncObject)
            {
                ClearOnScheduler();
            }
        }

        public void SyncAddRange(params T[] items)
        {
            lock (syncObject)
            {
                AddRangeOnScheduler(items);
            }
        }

        public void AddRange(params T[] items)
        {
            lock (syncObject)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsLimit.Value)
            {
                return;
            }

            try
            {
                CollectionChanged -= OnCollectionChanged;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (Items.Count > MaxSize.Value)
                    {
                        SyncRemoveAt(0);
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

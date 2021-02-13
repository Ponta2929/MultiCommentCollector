using Reactive.Bindings;
using System.Collections.Specialized;

namespace MCC.Core
{
    public class ListManagerBase<T>
    {
        /// <summary>
        /// リスト
        /// </summary>
        public ReactiveCollection<T> Items { get; private set; } = new();

        /// <summary>
        /// リストが保持できる件数です。
        /// </summary>
        public ReactivePropertySlim<int> MaxSize { get; set; } = new(1000);

        /// <summary>
        /// MaxSizeを有効化します。
        /// </summary>
        public ReactivePropertySlim<bool> IsLimit { get; set; } = new(true);

        public ListManagerBase()
            => Items.CollectionChanged += CollectionChanged;

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsLimit.Value)
                return;

            try
            {
                Items.CollectionChanged -= CollectionChanged;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (Items.Count > MaxSize.Value)
                    {
                        Items.RemoveAt(0);
                    }
                }
            }
            finally
            {
                Items.CollectionChanged += CollectionChanged;
            }
        }
    }
}

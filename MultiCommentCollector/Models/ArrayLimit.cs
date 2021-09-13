using Reactive.Bindings;
using System;

namespace MultiCommentCollector.Models
{
    [Serializable]
    public class ArrayLimit
    {
        /// <summary>
        /// MaxComments
        /// </summary>
        public ReactiveProperty<int> MaxComments { get; set; } = new(1000);

        /// <summary>
        /// MaxLogs
        /// </summary>
        public ReactiveProperty<int> MaxLogs { get; set; } = new(1000);
    }
}

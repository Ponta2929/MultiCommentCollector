using Reactive.Bindings;

namespace MultiCommentCollector.Models
{
    public class Pane
    {
        /// <summary>
        /// IsPaneOpen
        /// </summary>
        public ReactiveProperty<bool> IsPaneOpen { get; set; } = new(true);

        /// <summary>
        /// PaneWidth
        /// </summary>
        public ReactiveProperty<int> PaneWidth { get; set; } = new(250);
    }
}

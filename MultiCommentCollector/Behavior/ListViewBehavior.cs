using Microsoft.Xaml.Behaviors;
using MultiCommentCollector.Extensions;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace MultiCommentCollector.Behavior
{
    public class ListViewBehavior : Behavior<ListView>
    {
        private ScrollViewer scrollViewer;

        public int AutoResizeToItemColmunNumber { get; set; } = -1;

        protected override void OnAttached()
        {
            base.OnAttached();

            (AssociatedObject.Items as INotifyCollectionChanged).CollectionChanged += CollectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            (AssociatedObject.Items as INotifyCollectionChanged).CollectionChanged -= CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (scrollViewer is null)
            {
                scrollViewer = AssociatedObject.FindElement<ScrollViewer>();

                if (scrollViewer is not null)
                {
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (scrollToEnd)
                {
                    scrollViewer.ScrollToEnd();

                    if (AutoResizeToItemColmunNumber > -1)
                    {
                        var view = AssociatedObject.View as GridView;

                        view.Columns[AutoResizeToItemColmunNumber].Width = 0;
                        view.Columns[AutoResizeToItemColmunNumber].Width = double.NaN;
                    }
                }
            }
        }

        private bool scrollToEnd = false;

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset)
            {
                scrollToEnd = true;
            }
            else
            {
                scrollToEnd = false;
            }
        }
    }
}

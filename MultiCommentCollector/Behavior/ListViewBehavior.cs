using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using MultiCommentCollector.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MultiCommentCollector.Behavior
{
    public class ListViewBehavior : Behavior<ListView>
    {
        private ScrollViewer scrollViewer;

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
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (scrollToEnd)
                    scrollViewer.ScrollToEnd();
            }
        }

        private bool scrollToEnd = false;

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (scrollViewer.ScrollableHeight == scrollViewer.VerticalOffset)
                scrollToEnd = true;
            else
                scrollToEnd = false;

        }
    }
}

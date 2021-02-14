using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MultiCommentCollector.Behavior
{
    public class ListViewBehavior : Behavior<ListView>
    {

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
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[e.NewStartingIndex]);
            }
        }
    }
}

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MultiCommentCollector.Extensions
{
    public static class VisualTreeHelperExtension
    {
        /// <summary>
        /// VisualTreeを親側にたどって、
        /// 指定した型の要素を探す
        /// </summary>
        public static T FindAncestor<T>(this DependencyObject depObj) where T : DependencyObject
        {
            while (depObj is not null)
            {
                if (depObj is T target)
                {
                    return target;
                }

                depObj = VisualTreeHelper.GetParent(depObj);
            }

            return null;
        }

        public static T FindElement<T>(this DependencyObject rootElement) where T : DependencyObject
        {
            var q = new Queue<DependencyObject>();

            q.Enqueue(rootElement);

            while (q.Count != 0)
            {
                var e = q.Dequeue();

                if (e is T)
                {
                    return e as T;
                }

                var c = VisualTreeHelper.GetChildrenCount(e);

                for (var i = 0; i < c; ++i)
                {
                    q.Enqueue(VisualTreeHelper.GetChild(e, i));
                }
            }

            return null;
        }

        public static Rect GetBoundingRectWithMargin(this FrameworkElement element, FrameworkElement relativeTo)
        {
            var transform = element.TransformToVisual(relativeTo);
            var r = new Rect(0, 0, element.ActualWidth, element.ActualHeight);

            r = transform.TransformBounds(r);

            var m = element.Margin;

            return new Rect(new Point(r.Left - m.Left, r.Top - m.Top), new Point(r.Right + m.Right, r.Bottom + m.Bottom));
        }
    }
}

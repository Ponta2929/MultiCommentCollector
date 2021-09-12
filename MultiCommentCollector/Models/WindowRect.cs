using Reactive.Bindings;
using System;

namespace MultiCommentCollector.Models
{
    [Serializable]
    public class WindowRect
    {
        public ReactiveProperty<double> Width { get; set; } = new();

        public ReactiveProperty<double> Height { get; set; } = new();

        public ReactiveProperty<double> Left { get; set; } = new();

        public ReactiveProperty<double> Top { get; set; } = new();

        public WindowRect() { }

        public WindowRect(double width, double height, double left, double top)
            => (Width.Value, Height.Value, Left.Value, Top.Value) = (width, height, left, top);
    }
}

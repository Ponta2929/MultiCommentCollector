using MCC.Utility;
using System.Windows.Media;

namespace MultiCommentCollector.Extensions
{
    public static class ColorDataExtensions
    {
        public static Color ToArgb(this ColorData color)
            => Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);

        public static Color ToRgb(this ColorData color)
            => Color.FromArgb(255, (byte)color.R, (byte)color.G, (byte)color.B);

        public static ColorData FromColor(Color color)
            => new() { A = color.A, R = color.R, G = color.G, B = color.B };
    }
}

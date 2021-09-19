using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiCommentCollector.Converter
{
    internal class BackgroundColorConverter : IValueConverter
    {
        public static readonly BackgroundColorConverter Instance = new();

        private static readonly int Delta = 105;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                var isDark = IsDark(brush.Color);
                var newColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

                if (isDark)
                {
                    newColor.Color = Color.FromArgb(255, Range(brush.Color.R - Delta), Range(brush.Color.G - Delta), Range(brush.Color.B - Delta));
                }
                else
                {
                    newColor.Color = Color.FromArgb(255, Range(brush.Color.R + Delta), Range(brush.Color.G + Delta), Range(brush.Color.B + Delta));
                }

                newColor.Freeze();

                return newColor;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


        /// <summary>
        /// Determining Ideal Text Color Based on Specified Background Color
        /// http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name = "background">The background color.</param>
        /// <returns></returns>
        private static bool IsDark(Color background)
        {
            const int nThreshold = 86; //105;
            var bgDelta = System.Convert.ToInt32((background.R * 0.299) + (background.G * 0.587) + (background.B * 0.114));
            return (255 - bgDelta < nThreshold) ? true : false;
        }

        private static byte Range(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 255)
            {
                return 255;
            }

            return (byte)value;
        }
    }
}

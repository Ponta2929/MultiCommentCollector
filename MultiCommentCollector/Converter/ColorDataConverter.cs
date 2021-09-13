using MCC.Utility;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiCommentCollector.Converter
{
    public class ColorDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ColorData color)
            {
                return new SolidColorBrush(Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B));
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

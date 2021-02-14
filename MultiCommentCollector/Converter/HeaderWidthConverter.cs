using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiCommentCollector.Converter
{
    public class HeaderWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value is not double)
                {
                    return 0;
                }
            }

            var width = (double)values[0];

            for (var i = 1; i < values.Length; i++)
            {
                width -= (double)values[i];
            }

            return width > 0 ? width : 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiCommentCollector.Converter
{
    public class IsActiveToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool)
            {
                throw new NotImplementedException();
            }

            return (bool)value ? "有効" : "無効";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

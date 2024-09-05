using System;
using System.Globalization;
using System.Windows.Data;

namespace RssReader2.Views.Converters
{
    public class ZeroPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return value;
            }

            var num = (int)value;
            return num.ToString((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && int.TryParse(str, out var result))
            {
                return result;
            }

            return value;
        }
    }
}
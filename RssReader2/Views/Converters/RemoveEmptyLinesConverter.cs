using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RssReader2.Views.Converters
{
    public class RemoveEmptyLinesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text)
            {
                return value;
            }

            // 空行を削除して新しいテキストを生成
            var lines = text.Split(new[] { Environment.NewLine, }, StringSplitOptions.None);
            var nonEmptyLines = string.Join(Environment.NewLine, lines.Where(line => !string.IsNullOrWhiteSpace(line)));
            return nonEmptyLines;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
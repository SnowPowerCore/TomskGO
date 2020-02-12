using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace TomskGO.Core.Converters
{
    class DetagifyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var str = (string)value;
                var regex = new Regex(@"#\w+");
                var tags = regex.Matches(str);
                foreach (Match m in tags)
                {
                    str = str.Replace(m.Value + " ", "");
                    str = str.Replace(m.Value, "");
                }
                var newLineRegex = new Regex(Regex.Escape("\n"));
                str = newLineRegex.Replace(str, "", 2);
                return str;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            null;
    }
}

using System;
using System.Globalization;
using Xamarin.Forms;

namespace TomskGO.Converters
{
    class BoolToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var b = (bool)value;
                return b ? Color.FromHex("#d3f5e4") : Application.Current.Resources["chipBackgroundColor"];
            }
            return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }
}

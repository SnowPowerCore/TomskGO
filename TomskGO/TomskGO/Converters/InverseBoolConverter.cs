using System;
using System.Globalization;
using Xamarin.Forms;

namespace TomskGO.Core.Converters
{
    class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return (bool)value;
            }
            return !(bool)value;
        }
    }
}
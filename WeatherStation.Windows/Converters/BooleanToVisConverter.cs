using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WeatherStation.Windows.Converters
{
    public class BooleanToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool parsed = false;

            if (value is bool)
            {
                parsed = (bool)value;
            }
            else
            {
                parsed = System.Convert.ToBoolean(value);
            }

            return (parsed) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vis = (Visibility)value;

            if (vis == Visibility.Collapsed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

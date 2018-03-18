using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WeatherStation.Core.Forecasts;

namespace WeatherStation.Windows.Converters
{
    public class PackIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeatherCodes)
            {
                var icon = (WeatherCodes)value;
                switch (icon)
                {
                    case WeatherCodes.thunderstorm:
                    case WeatherCodes.thunderstorm_with_drizzle:
                    case WeatherCodes.thunderstorm_with_heavy_rain:
                    case WeatherCodes.thunderstorm_with_light_drizzle:
                    case WeatherCodes.thunderstorm_with_rain:
                    case WeatherCodes.ragged_thunderstorm:
                    case WeatherCodes.light_thunderstorm:
                    case WeatherCodes.heavy_thunderstorm:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherLightning;
                    case WeatherCodes.hail:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherHail;
                    case WeatherCodes.fog:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherFog;
                    case WeatherCodes.overcast_clouds:
                    case WeatherCodes.scattered_clouds:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherCloudy;
                    case WeatherCodes.broken_clouds:
                    case WeatherCodes.few_clouds:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherPartlycloudy;
                    case WeatherCodes.drizzle_rain:
                    case WeatherCodes.light_intensity_drizzle_rain:
                    case WeatherCodes.light_intensity_drizzle:
                    case WeatherCodes.shower_rain_and_drizzle:
                    case WeatherCodes.shower_rain:
                    case WeatherCodes.light_rain:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherRainy;
                    case WeatherCodes.extreme_rain:
                    case WeatherCodes.freezing_rain:
                    case WeatherCodes.heavy_intensity_drizzle_rain:
                    case WeatherCodes.heavy_intensity_drizzle:
                    case WeatherCodes.heavy_intensity_shower_rain:
                    case WeatherCodes.heavy_intensity_rain:
                    case WeatherCodes.very_heavy_rain:
                    case WeatherCodes.moderate_rain:
                    case WeatherCodes.heavy_shower_rain_and_drizzle:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherPouring;
                    case WeatherCodes.snow:
                    case WeatherCodes.shower_snow:
                    case WeatherCodes.heavy_snow:
                    case WeatherCodes.heavy_shower_snow:
                    case WeatherCodes.light_snow:
                    case WeatherCodes.light_shower_snow:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherSnowy;
                    case WeatherCodes.rain_and_snow:
                    case WeatherCodes.light_rain_and_snow:
                    case WeatherCodes.sleet:
                    case WeatherCodes.shower_sleet:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherSnowyRainy;
                    case WeatherCodes.clear_sky:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherSunny;
                    case WeatherCodes.high_wind:
                        return MaterialDesignThemes.Wpf.PackIconKind.WeatherWindy; 
                    default:
                        break;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

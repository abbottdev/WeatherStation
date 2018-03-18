using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Forecasts;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;
using WeatherStation.Windows.ViewModels;

namespace WeatherStation.Windows.Designer
{
    public class WeatherStationDesigner : ViewModels.WeatherStationViewModel
    {
        private static Location Location;
        private static double temperature;

        static WeatherStationDesigner()
        {
            WeatherStationDesigner.Location = new Location(0, 0, "Leeds, UK");
            temperature = 239;
        }

        static TodaysForecast GetTodaysForecast()
        {
            return new TodaysForecast(WeatherStationDesigner.Location, temperature, "Sunny", "", WeatherCodes.clear_sky, DateTime.Today, 3, 1080, 10, DateTime.Today.AddHours(6), DateTime.Today.AddHours(18));
        }
        public WeatherStationDesigner() : base(null, GetTodaysForecast(), null, null)
        {
        }

        public override IEnumerable<DayForecastModel> Forecasts => GenerateForecasts();

        private IEnumerable<DayForecastModel> GenerateForecasts()
        {
            var forecasts = new List<DayForecastModel>();
            for (var i = 1; i < 7; i++)
            {
                forecasts.Add(new DayForecastModel(GenerateSunnyForecast(DateTime.Today.AddDays(i)), null, null));
            }

            return forecasts;
        }

        private Forecast GenerateSunnyForecast(DateTime date)
        {
            return new Forecast(Location, temperature, "Sunny", "", WeatherCodes.clear_sky, date);
        }
    }
}

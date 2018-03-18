using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;
using WeatherStation.Windows.ViewModels;

namespace WeatherStation.Windows.Designer
{
    internal class DetailedForecastViewModelDesigner : DetailedForecastViewModel
    {
        private List<DayForecastModel> hourlyDesigner;

        public DetailedForecastViewModelDesigner() : base(null, new Location(0, 0, "Location"), DateTime.Now, null)
        {

            this.hourlyDesigner = new List<DayForecastModel>();
            int hour = 0;
            DateTime start = DateTime.Today;

            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));
            this.hourlyDesigner.Add(AddHour(ref hour, start));


        }

        private DayForecastModel AddHour(ref int hour, DateTime start)
        {
            hour++;
            return new DayForecastModel(new Core.Forecasts.Forecast(
                 new Location(0, 0, "Location"),
                 283.15,
                 "Snow Showers", "", Core.Forecasts.WeatherCodes.shower_snow, start.AddHours(hour)), null, null);
        }

        public override IEnumerable<DayForecastModel> HourlyForecast => hourlyDesigner;

        public DetailedForecastViewModelDesigner(AppViewModel screen, Location location, DateTime date, IWeatherService service) : base(screen, location, date, service)
        {
        }
    }
}

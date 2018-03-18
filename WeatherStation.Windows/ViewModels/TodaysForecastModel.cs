using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Forecasts;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.ViewModels
{
    public class TodaysForecastModel : DayForecastModel
    {

        private int humidityPercent;
        private int pressure;
        private DateTime sunrise;
        private DateTime sunset;
        private int windSpeed;

        public DateTime Sunrise
        {
            get { return sunrise; }
            set { this.RaiseAndSetIfChanged(ref sunrise, value); }
        }

        public DateTime Sunset
        {
            get { return sunset; }
            set { this.RaiseAndSetIfChanged(ref sunset, value); }
        }


        public int Humidity
        {
            get { return humidityPercent; }
            set { this.RaiseAndSetIfChanged(ref humidityPercent, value); }
        }

        public int Pressure
        {
            get { return pressure; }
            set { this.RaiseAndSetIfChanged(ref pressure, value); }
        }

        public int WindSpeed
        {
            get { return windSpeed; }
            set { this.RaiseAndSetIfChanged(ref windSpeed, value); }
        }



        public TodaysForecastModel(TodaysForecast weather, AppViewModel app, IWeatherService service) : base(weather, app, service)
        {
            this.Humidity = weather.Humidity;
            this.Pressure = weather.Pressure;
            this.Sunrise = weather.Sunrise;
            this.Sunset = weather.Sunset;
            this.WindSpeed = weather.WindSpeed;
            //wiunnd.speed

        }
    }
}

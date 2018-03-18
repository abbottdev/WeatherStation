using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Forecasts;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.ViewModels
{
    public class DayForecastModel : ReactiveObject
    {

        private DateTime forecastDate;
        private string temperature;
        private string weatherDescription;
        private string weatherIconUrl;
        private WeatherCodes weatherCode;
        private ReactiveCommand<Unit, IObservable<IRoutableViewModel>> viewCommand;

        public ReactiveCommand ViewCommand => this.viewCommand;

        public WeatherCodes WeatherCode
        {
            get { return weatherCode; }
            set { this.RaiseAndSetIfChanged(ref this.weatherCode, value); }
        }

        public string Temperature
        {
            get { return temperature; }
            set { this.RaiseAndSetIfChanged(ref this.temperature, value); }
        }

        public DateTime ForecastDate
        {
            get { return forecastDate; }
            set { this.RaiseAndSetIfChanged(ref this.forecastDate, value); }
        }

        public string WeatherDescription
        {
            get { return weatherDescription; }
            set { this.RaiseAndSetIfChanged(ref this.weatherDescription, value); }
        }

        public string WeatherIconUrl
        {
            get { return weatherIconUrl; }
            set { this.RaiseAndSetIfChanged(ref this.weatherIconUrl, value); }
        }
        public DayForecastModel(Forecast weather, AppViewModel app, IWeatherService weatherService)
        {
            this.viewCommand = ReactiveCommand.Create(() => app.Router.Navigate.Execute(new DetailedForecastViewModel(app, weather.Location, weather.ForecastDate, weatherService)));

            UpdateFromForecast(weather);
        }

        private void UpdateFromForecast(Forecast forecast)
        {
            bool isMetric = RegionInfo.CurrentRegion.IsMetric;

            //Convert to either Celcius or Fahrenheit based on regional settings.
            double regionalTemperature = (isMetric) ? forecast.Temperature - 273.15 : forecast.Temperature - 9 / 5 - 459.67;

            this.Temperature = $"{regionalTemperature:F0} " + ((isMetric) ? "°C" : "°F");

            this.ForecastDate = forecast.ForecastDate;
            this.WeatherDescription = forecast.WeatherDescription;
            this.WeatherIconUrl = forecast.WeatherIconUrl;
            this.WeatherCode = forecast.WeatherCode;
        }

    }
}

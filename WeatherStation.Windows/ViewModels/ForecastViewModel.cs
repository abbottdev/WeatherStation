using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Forecasts;
using ReactiveUI;
using WeatherStation.Core.Services;
using System.Reactive;

namespace WeatherStation.Windows.ViewModels
{
    public class ForecastViewModel : ReactiveObject, IRoutableViewModel
    {
        private Core.Locations.Location location;
        private double temperature;
        private IWeatherService service;
        private ReactiveCommand<Unit, Forecast> refreshWeatherCommand;
        private DateTime forecastDate;

        public ReactiveCommand RefreshWeather
        {
            get
            {
                return this.refreshWeatherCommand;
            }
        }

        public double Temperature
        {
            get { return temperature; }
            set { this.RaiseAndSetIfChanged(ref this.temperature, value); }
        }

        public DateTime ForecastDate
        {
            get { return forecastDate; }
            set { this.RaiseAndSetIfChanged(ref this.forecastDate, value); }
        }

        public string UrlPathSegment => "forecasts/" + forecastDate.ToString();

        public IScreen HostScreen { get; } 

        public ForecastViewModel(IScreen screen, Core.Locations.Location location, IWeatherService service)
        {
            this.HostScreen = screen;
            this.service = service;
            this.location = location;

            this.refreshWeatherCommand = ReactiveCommand.CreateFromTask(async () => await service.GetTodaysWeatherAsync(location));
            this.refreshWeatherCommand.Subscribe(forecast => this.UpdateFromForecast(forecast));

            this.WhenNavigatedTo(() => Task.Run(async () =>
            {
                await this.refreshWeatherCommand.Execute();
            }));
        }

        public ForecastViewModel(IScreen screen, Forecast forecast, Core.Locations.Location location, IWeatherService service) : this(screen, location, service)
        {
            UpdateFromForecast(forecast);
        }

        private void UpdateFromForecast(Forecast forecast)
        {
            this.Temperature = forecast.Temperature;
            this.ForecastDate = forecast.ForecastDate;
        }

    }
}

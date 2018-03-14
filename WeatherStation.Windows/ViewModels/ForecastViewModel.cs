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
using System.Globalization;

namespace WeatherStation.Windows.ViewModels
{
    public class ForecastViewModel : ReactiveObject, IRoutableViewModel
    {
        private Core.Locations.Location location;
        private IWeatherService service;
        private ReactiveCommand<Unit, TodaysForecast> refreshTodaysWeather;
        private ReactiveCommand<Unit, IEnumerable<ForecastModel>> loadForecast;
        private ObservableAsPropertyHelper<IEnumerable<ForecastModel>> forecasts;
        private TodaysForecastModel today;
        private ObservableAsPropertyHelper<bool> isBusy;

        public ReactiveCommand RefreshWeather
        {
            get
            {
                return this.refreshTodaysWeather;
            }
        }
        
        public TodaysForecastModel Today
        {
            get { return today; }
            set { this.RaiseAndSetIfChanged(ref this.today, value); }
        }

        public IEnumerable<ForecastModel> Forecasts
        {
            get
            {
                return this.forecasts.Value;
            }
        }


        public string LocationName
        {
            get
            {
                return this.location.DisplayName;
            }
        }


        public string UrlPathSegment => "forecast/";
        public bool IsBusy => this.isBusy.Value;

        public IScreen HostScreen { get; }

        public ForecastViewModel(IScreen screen, Core.Locations.Location location, IWeatherService service)
        {
            this.HostScreen = screen;
            this.service = service;
            this.location = location;

            this.refreshTodaysWeather = ReactiveCommand.CreateFromTask(async () => await service.GetTodaysWeatherAsync(location));

            this.loadForecast = ReactiveCommand.CreateFromTask(async () =>
            {
                var forecasts = await service.GetWeatherForecastAsync(location);

                return forecasts.Select(f => new ForecastModel(f));
            });

            this.refreshTodaysWeather.Subscribe(weather => this.Today = new TodaysForecastModel(weather));
            this.forecasts = this.loadForecast.ToProperty(this, vm => vm.Forecasts);
            
            Observable
                .CombineLatest(this.refreshTodaysWeather.IsExecuting, this.loadForecast.IsExecuting, (today, forecast) => today || forecast)
                .ToProperty(this, vm => vm.IsBusy, out this.isBusy);

            this.WhenNavigatedTo(() => Task.Run(async () =>
            {
                await this.refreshTodaysWeather.Execute();
                await this.loadForecast.Execute();
            }));
        }

        public ForecastViewModel(IScreen screen, TodaysForecast forecast, Core.Locations.Location location, IWeatherService service) : this(screen, location, service)
        {
            Today = new TodaysForecastModel(forecast);
        }

    }
}

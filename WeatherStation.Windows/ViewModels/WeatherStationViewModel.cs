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
using System.ComponentModel;
using System.Windows;

namespace WeatherStation.Windows.ViewModels
{
    public class WeatherStationViewModel : ReactiveObject, IRoutableViewModel
    {
        private ObservableAsPropertyHelper<IEnumerable<DayForecastModel>> forecasts;
        private ObservableAsPropertyHelper<bool> isBusy;
        private ReactiveCommand<Unit, IEnumerable<DayForecastModel>> loadForecast;
        private Core.Locations.Location location;
        private ReactiveCommand<Unit, TodaysForecast> refreshTodaysWeather;
        private IWeatherService service;
        private TodaysForecastModel today;

        public WeatherStationViewModel(AppViewModel screen, Core.Locations.Location location, IWeatherService service)
        {
            this.HostScreen = screen;
            this.service = service;
            this.location = location;
#if DEBUG
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            {
                
            }
            else
            {
#endif


            this.refreshTodaysWeather = ReactiveCommand.CreateFromTask(async () => await service.GetTodaysWeatherAsync(location));

            this.loadForecast = ReactiveCommand.CreateFromTask(async () =>
            {
                var forecasts = await service.GetWeatherForecastAsync(location);

                return forecasts.Select(f => new DayForecastModel(f, screen, service));
            });

            this.refreshTodaysWeather.Subscribe(weather => this.Today = new TodaysForecastModel(weather, screen, service));
            this.forecasts = this.loadForecast.ToProperty(this, vm => vm.Forecasts);

            Observable
                .CombineLatest(this.refreshTodaysWeather.IsExecuting, this.loadForecast.IsExecuting, (today, forecast) => today || forecast)
                .ToProperty(this, vm => vm.IsBusy, out this.isBusy);

            this.WhenNavigatedTo(() => Task.Run(async () =>
            {
                await this.refreshTodaysWeather.Execute();
                await this.loadForecast.Execute();
            }));

#if DEBUG
            }
#endif
        }

        public WeatherStationViewModel(AppViewModel screen, TodaysForecast forecast, Core.Locations.Location location, IWeatherService service) : this(screen, location, service)
        {
            Today = new TodaysForecastModel(forecast, screen, service);
        }

        public virtual IEnumerable<DayForecastModel> Forecasts
        {
            get
            {
                return this.forecasts.Value;
            }
        }

        public IScreen HostScreen { get; }

        public bool IsBusy => this.isBusy.Value;

        public string LocationName
        {
            get
            {
                return this.location.DisplayName;
            }
        }

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
        public string UrlPathSegment => "forecast/";
    }
}

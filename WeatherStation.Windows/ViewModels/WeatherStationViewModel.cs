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
using WeatherStation.Core.Health;

namespace WeatherStation.Windows.ViewModels
{
    public class WeatherStationViewModel : ReactiveObject, IRoutableViewModel
    {
        private ObservableAsPropertyHelper<IEnumerable<DayForecastModel>> forecasts;
        private ObservableAsPropertyHelper<bool> isBusy;
        private ReactiveCommand<Unit, IEnumerable<DayForecastModel>> forecastCommand;
        private Core.Locations.Location location;
        private ReactiveCommand<Unit, TodaysForecast> refreshTodaysWeather;
        private ObservableAsPropertyHelper<TodaysForecastModel> todaysWeatherProperty;
        private IWeatherService service;
        private ObservableAsPropertyHelper<IEnumerable<ConditionViewModel>> conditionsAffectedByWeatherProperty;
        private ObservableAsPropertyHelper<bool> hasConditions;

        public WeatherStationViewModel(AppViewModel screen, Core.Locations.Location location, IWeatherService service, IHealthService healthService)
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
                this.todaysWeatherProperty = this.refreshTodaysWeather.Select(weather => new TodaysForecastModel(weather, screen, service)).ToProperty(this, vm => vm.Today);

                this.forecastCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var forecasts = await service.GetWeatherForecastAsync(location);

                    return forecasts.Select(f => new DayForecastModel(f, screen, service));
                });

                this.forecasts = this.forecastCommand.ToProperty(this, vm => vm.Forecasts);

                var refreshConditionsAffectedByWeatherCommand = ReactiveCommand.CreateFromTask(async (TodaysForecast weather) =>
                {
                    var conditions = await healthService.GetConditionsAffectedByWeatherAsync(weather.WeatherCode, weather.Temperature);

                    return conditions.Select(c => new ConditionViewModel(screen, c.Id, c.Name, healthService));
                });

                this.conditionsAffectedByWeatherProperty = refreshConditionsAffectedByWeatherCommand.ToProperty(this, vm => vm.ConditionsAffectedByWeather);

                this.refreshTodaysWeather.InvokeCommand(refreshConditionsAffectedByWeatherCommand);

                //Create a busy property to show the user we're busy doing something.
                Observable
                    .CombineLatest(this.refreshTodaysWeather.IsExecuting, this.forecastCommand.IsExecuting, refreshConditionsAffectedByWeatherCommand.IsExecuting, (today, forecast, conditions) => today || forecast || conditions)
                    .ToProperty(this, vm => vm.IsBusy, out this.isBusy);

                this.hasConditions = this.WhenAnyValue(vm => vm.ConditionsAffectedByWeather).Select(items => items != null && items.Count() > 0).ToProperty(this, vm => vm.HasConditionsAffectedByWeather);

                this.WhenNavigatedTo(() => Task.Run(async () =>
                {
                    await this.refreshTodaysWeather.Execute();
                    await this.forecastCommand.Execute();
                }));

#if DEBUG
            }
#endif
        }


        public virtual IEnumerable<DayForecastModel> Forecasts
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

        public ReactiveCommand RefreshWeather
        {
            get
            {
                return this.refreshTodaysWeather;
            }
        }

        public virtual TodaysForecastModel Today
        {
            get { return todaysWeatherProperty.Value; }
        }

        public IEnumerable<ConditionViewModel> ConditionsAffectedByWeather => this.conditionsAffectedByWeatherProperty.Value;
        public bool HasConditionsAffectedByWeather => this.hasConditions.Value;

        public bool IsBusy => this.isBusy.Value;

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "forecast/";

    }
}

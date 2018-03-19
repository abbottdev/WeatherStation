using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.ViewModels
{
    public class DetailedForecastViewModel : ReactiveObject, IRoutableViewModel
    {
        private DateTime date;
        private ObservableAsPropertyHelper<IEnumerable<DayForecastModel>> hourlyForecast;
        private ReactiveCommand<Unit, IEnumerable<DayForecastModel>> refreshForecast;


        public DetailedForecastViewModel(AppViewModel screen, Location location, DateTime date, IWeatherService service)
        {
            this.HostScreen = screen;
            this.Date = date;

            if (screen != null)
            {
                this.refreshForecast = ReactiveCommand.CreateFromTask(async () =>
                {
                    var hourly = await service.GetHourlyWeatherForDateAsync(location, date, TimeSpan.FromDays(1));

                    return hourly.Select(h => new DayForecastModel(h, screen, service));
                });

                this.hourlyForecast = refreshForecast.ToProperty(this, vm => vm.HourlyForecast);

                this.WhenNavigatedTo(() => Task.Run(async () => await refreshForecast.Execute()));

                this.BackCommand = screen.Router.NavigateBack;
            }
        }

        public DateTime Date
        {
            get { return date; }
            set { this.RaiseAndSetIfChanged(ref date, value); }
        }

        public ReactiveCommand BackCommand { get; }
        public IScreen HostScreen { get; }
        public virtual IEnumerable<DayForecastModel> HourlyForecast => this.hourlyForecast.Value;
        public string UrlPathSegment => "forecasts/" + this.date.ToShortDateString();
    }
}

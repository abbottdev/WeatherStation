using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WeatherStation.Core.Services;
using WeatherStation.Services.Health;
using WeatherStation.Services.OpenWeatherMap;
using WeatherStation.Windows.Services;
using WeatherStation.Windows.Views;

namespace WeatherStation.Windows.ViewModels
{
    public class AppViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; protected set; }
        public ReactiveCommand<Unit, Unit> ResetSettings { get; private set; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public AppViewModel()
        {
            Router = new RoutingState();

            RegisterDependencies();

            this.ResetSettings = ReactiveCommand.CreateFromTask(async () =>
            {
                var service = Locator.Current.GetService<IConfigurationService>();

                await service.ChangeSavedLocationAsync(null);
                this.LoadCommand.Execute().Subscribe();
            });

            this.LoadCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var service = Locator.Current.GetService<IConfigurationService>();

                var storedLocation = await service.GetSavedLocationAsync();

                if (storedLocation != null)
                {
                    //Navigate to the forecast view.
                    //Determine the correct view model to navigate to based on configuration.
                    this
                     .Router
                     .NavigateAndReset
                     .Execute(new WeatherStationViewModel(this, storedLocation, Locator.Current.GetService<IWeatherService>(), Locator.Current.GetService<IHealthService>()))
                     .Subscribe();
                }
                else
                {
                    this
                     .Router
                     .NavigateAndReset
                     .Execute(new SelectLocationViewModel(this, Locator.Current.GetService<IConfigurationService>(), Locator.Current.GetService<IGeocodeService>()))
                     .Subscribe();
                }
            });

            Router.Navigate.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message));

            this.LoadCommand.Execute().Subscribe();
        }

        private void RegisterDependencies()
        {
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            string healthPortalEndpoint = "http://weatherstationhealthportal20180319111551.azurewebsites.net";

            //Todo: register views.
            //Locator.CurrentMutable.Register(() => new UpcomingMoviesListView(), typeof(IViewFor<UpcomingMoviesListViewModel>));
            //Locator.CurrentMutable.Register(() => new UpcomingMoviesCellView(), typeof(IViewFor<UpcomingMoviesCellViewModel>));
            Locator.CurrentMutable.Register(() => new WeatherStationView(), typeof(IViewFor<WeatherStationViewModel>));
            Locator.CurrentMutable.Register(() => new SelectLocationView(), typeof(IViewFor<SelectLocationViewModel>));
            Locator.CurrentMutable.Register(() => new DetailedForecastView(), typeof(IViewFor<DetailedForecastViewModel>));
            Locator.CurrentMutable.Register(() => new ConditionView(), typeof(IViewFor<ConditionViewModel>));

            Locator.CurrentMutable.Register(() => new AkavacheConfigurationService(), typeof(IConfigurationService));
            Locator.CurrentMutable.Register(() => new OWMWeatherService(), typeof(IWeatherService));
            Locator.CurrentMutable.Register(() => new GoogleGeocoderService(), typeof(IGeocodeService));
            Locator.CurrentMutable.Register(() => new HealthPortalService(healthPortalEndpoint), typeof(IHealthService));
        }
    }
}

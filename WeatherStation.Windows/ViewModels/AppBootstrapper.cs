using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WeatherStation.Core.Services;
using WeatherStation.Windows.Views;

namespace WeatherStation.Windows.ViewModels
{
    public class AppViewModel :  ReactiveObject, IScreen
    {
        public RoutingState Router { get; protected set; }


        public AppViewModel()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            
            //Todo: register views.
            //Locator.CurrentMutable.Register(() => new UpcomingMoviesListView(), typeof(IViewFor<UpcomingMoviesListViewModel>));
            //Locator.CurrentMutable.Register(() => new UpcomingMoviesCellView(), typeof(IViewFor<UpcomingMoviesCellViewModel>));
            Locator.CurrentMutable.Register(() => new ForecastView(), typeof(IViewFor<ForecastViewModel>));

            //Locator.CurrentMutable.Register(() => new Cache(), typeof(ICache));

            Locator.CurrentMutable.Register(() => new Services.OpenWeatherMap.WeatherService(), typeof(IWeatherService));

            //Determine the correct view model to navigate to based on configuration.
            this
             .Router
             .NavigateAndReset
             .Execute(new ForecastViewModel(this, new Core.Locations.Location(54, 54, "Home"), Locator.Current.GetService<IWeatherService>()))
             .Subscribe();
        }
        

    }
}

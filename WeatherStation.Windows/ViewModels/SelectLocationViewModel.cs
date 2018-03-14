using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.ViewModels
{
    public class SelectLocationViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "select-location";

        private IConfigurationService configurationService;
        private IGeocodeService geocoder;
        string searchTerm;
        public string SearchTerm
        {
            get { return searchTerm; }
            set { this.RaiseAndSetIfChanged(ref searchTerm, value); }
        }

        public ReactiveCommand<string, IEnumerable<Location>> ExecuteSearch { get; protected set; }
        public ReactiveCommand<Unit, Unit> SaveSelectedLocation { get; private set; }

        ObservableAsPropertyHelper<IEnumerable<Location>> searchResults;
        public IEnumerable<Location> SearchResults => searchResults.Value;

        private Location selectedLocation;
        private ObservableAsPropertyHelper<bool> isSearching;

        public Location SelectedLocation
        {
            get { return selectedLocation; }
            set { this.RaiseAndSetIfChanged(ref this.selectedLocation, value); }
        }


        public IScreen HostScreen { get; }
        public bool IsSearching => this.isSearching.Value;

        public SelectLocationViewModel(AppViewModel screen, IConfigurationService configurationService, IGeocodeService geocoder)
        {
            this.HostScreen = screen;

            this.configurationService = configurationService;
            this.geocoder = geocoder;

            ExecuteSearch = ReactiveCommand.CreateFromTask<string, IEnumerable<Location>>(
                async searchTerm => await geocoder.SearchForLocationsAsync(searchTerm)
            );

            this.SaveSelectedLocation = ReactiveCommand.CreateFromTask(async () =>
            {
                await configurationService.ChangeSavedLocationAsync(this.SelectedLocation);

                //Attempt to re-load app
                screen.LoadCommand.Execute().Subscribe();

            }, this.WhenAny(vm => vm.SelectedLocation, location => location.GetValue() != null));

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);

            this.ExecuteSearch.IsExecuting.ToProperty(this, x => x.IsSearching, out this.isSearching, false);

            searchResults = ExecuteSearch.ToProperty(this, x => x.SearchResults, new List<Location>());
        }

        
    }
}

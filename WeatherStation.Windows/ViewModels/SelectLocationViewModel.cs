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

        string searchTerm;
        public string SearchTerm
        {
            get { return searchTerm; }
            set { this.RaiseAndSetIfChanged(ref searchTerm, value); }
        }

        public ReactiveCommand<string, List<Location>> ExecuteSearch { get; protected set; }
        public ReactiveCommand<Unit, Unit> SaveSelectedLocation { get; private set; }

        ObservableAsPropertyHelper<List<Location>> _SearchResults;
        public List<Location> SearchResults => _SearchResults.Value;

        private Location selectedLocation;
        private ObservableAsPropertyHelper<bool> isSearching;

        public Location SelectedLocation
        {
            get { return selectedLocation; }
            set { this.RaiseAndSetIfChanged(ref this.selectedLocation, value); }
        }


        public IScreen HostScreen { get; }
        public bool IsSearching => this.isSearching.Value;

        public SelectLocationViewModel(AppViewModel screen, IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
            this.HostScreen = screen;

            ExecuteSearch = ReactiveCommand.CreateFromTask<string, List<Location>>(
                searchTerm => GeocodeAddress(searchTerm)
            );

            this.SaveSelectedLocation = ReactiveCommand.CreateFromTask(async () =>
            {
                await configurationService.ChangeSavedLocationAsync(this.SelectedLocation);

                //Attempt to re-load
                screen.LoadCommand.Execute().Subscribe();

            }, this.WhenAny(vm => vm.SelectedLocation, location => location.GetValue() != null));

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);

            this.ExecuteSearch.IsExecuting.ToProperty(this, x => x.IsSearching, out this.isSearching, false);

            _SearchResults = ExecuteSearch.ToProperty(this, x => x.SearchResults, new List<Location>());
        }

        private Task<List<Location>> GeocodeAddress(string searchTerm)
        {
            var locations = new Location[] { new Location(54, 54, "Russia"),
                    new Location(53.800726, -1.5492207, "Leeds, UK"),
                    new Location(53.8519224,-1.6841584, "Rawdon, Leeds, UK"),
                    new Location(53.5876143,-2.5370859, "Bolton, UK")
                }.ToList();


            return Task.FromResult(locations.Where(l => l.DisplayName.Contains(searchTerm)).ToList());
        }
    }
}

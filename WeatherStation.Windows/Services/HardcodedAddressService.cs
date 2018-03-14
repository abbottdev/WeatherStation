using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.Services
{
    public class HardcodedAddressService : IGeocodeService
    {
        public Task<IEnumerable<Location>> SearchForLocationsAsync(string input)
        {
            var locations = new Location[] {
                    new Location(53.800726, -1.5492207, "Leeds, UK"),
                    new Location(53.8519224,-1.6841584, "Rawdon, Leeds, UK"),
                    new Location(53.5876143,-2.5370859, "Bolton, UK")
                }.ToList();
            
            return Task.FromResult(locations.Where(l => l.DisplayName.Contains(input)));
        }
    }
}

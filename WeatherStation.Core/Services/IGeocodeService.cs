using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Core.Services
{
    public interface IGeocodeService
    {

        Task<IEnumerable<Locations.Location>> SearchForLocationsAsync(string input);

    }
}

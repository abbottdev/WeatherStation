using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;

namespace WeatherStation.Core.Services
{
    public interface IConfigurationService
    {

        Task<Location> GetSavedLocationAsync();
        Task ChangeSavedLocationAsync(Location location);
        
    }
}

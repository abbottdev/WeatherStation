using Akavache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;
using System.Reactive.Linq;

namespace WeatherStation.Windows.Services
{
    class AkavacheConfigurationService : IConfigurationService
    {
        public async Task ChangeSavedLocationAsync(Location location)
        {
            await BlobCache.LocalMachine.InsertObject<Location>("saved-location", location);
        }

        public async Task<Location> GetSavedLocationAsync()
        {
            if ((await BlobCache.LocalMachine.GetObjectCreatedAt<Location>("saved-location")).HasValue)
            {
                return await BlobCache.LocalMachine.GetObject<Location>("saved-location");
            } else
            {
                return null;
            }
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.Services
{
    public class GoogleGeocoderService : IGeocodeService
    {
        private const string G_API_KEY = "AIzaSyC9ZLg7kDqBqCKSZWPlrl0CSxNO2fcH_uE";

        public async Task<IEnumerable<Location>> SearchForLocationsAsync(string input)
        {
            HttpClient client = new HttpClient();
            List<Location> results = new List<Location>();

            string urlEncodedInput = Uri.EscapeDataString(input);

            client.BaseAddress = new Uri("https://maps.googleapis.com");

            var response = await client.GetAsync($"maps/api/geocode/json?address={urlEncodedInput}&key={G_API_KEY}");

            response.EnsureSuccessStatusCode();

            JObject json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

            if (json["status"].Value<string>() == "OK")
            {
                foreach (var item in json["results"].Children())
                {
                    results.Add(new Location(
                        item["geometry"]["location"].Value<double>("lat"),
                        item["geometry"]["location"].Value<double>("lng"),
                        item.Value<string>("formatted_address")));
                }
            }

            return results;
        }
        
    }
}



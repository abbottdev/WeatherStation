using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Services;
using WeatherStation.Core.Forecasts;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;

namespace WeatherStation.Services.OpenWeatherMap
{
    public class OWMWeatherService : IWeatherService
    {
        private const string apiKey = "3fe2f0443982c01e62473a1e96a7f030";

        public OWMWeatherService()
        {

        }

        public async Task<TodaysForecast> GetTodaysWeatherAsync(Location location)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");

            client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

            var response = await client.GetAsync($"weather?lat={location.Latitude}&lon={location.Longitude}&appid={apiKey}");

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(responseBody);

                return new TodaysForecast(
                    location,
                    json["main"].Value<double>("temp"),
                    json["weather"].First.Value<string>("main"),
                    $"http://openweathermap.org/img/w/{ json["weather"].First.Value<string>("icon")}.png",
                    (WeatherCodes)json["weather"].First.Value<int>("id"),
                    DateTime.Today,
                    json["main"].Value<int>("humidity"),
                    json["main"].Value<int>("pressure"),
                    json["wind"].Value<int>("speed"),
                    json["sys"].Value<long>("sunrise").ToDateTimeFromUnixEpoch(),
                    json["sys"].Value<long>("sunset").ToDateTimeFromUnixEpoch()
                );
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }



            //example response:
            //{
            //    "coord": {
            //        "lon": 139.01,
            //    "lat": 35.02
            //                },
            //  "weather": [
            //    {
            //      "id": 800,
            //      "main": "Clear",
            //      "description": "clear sky",
            //      "icon": "01n"
            //    }
            //  ],
            //  "base": "stations",
            //  "main": {
            //    "temp": 285.514,
            //    "pressure": 1013.75,
            //    "humidity": 100,
            //    "temp_min": 285.514,
            //    "temp_max": 285.514,
            //    "sea_level": 1023.22,
            //    "grnd_level": 1013.75
            //  },
            //  "wind": {
            //    "speed": 5.52,
            //    "deg": 311
            //  },
            //  "clouds": {
            //    "all": 0
            //  },
            //  "dt": 1485792967,
            //  "sys": {
            //    "message": 0.0025,
            //    "country": "JP",
            //    "sunrise": 1485726240,
            //    "sunset": 1485763863
            //  },
            //  "id": 1907296,
            //  "name": "Tawarano",
            //  "cod": 200
            //}


        }

        public async Task<IEnumerable<Forecast>> GetWeatherForecastAsync(Location location)
        {
            HttpClient client = new HttpClient();
            List<Forecast> forecasts = new List<Forecast>();

            client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");

            client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

            var response = await client.GetAsync($"forecast?lat={location.Latitude}&lon={location.Longitude}&appid={apiKey}");

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(responseBody);

                foreach (var item in json["list"])
                {
                    forecasts.Add(
                        new Forecast(
                            location,
                            item["main"].Value<double>("temp"),
                            item["weather"].First.Value<string>("main"),
                             $"http://openweathermap.org/img/w/{ item["weather"].First.Value<string>("icon")}.png",
                             (WeatherCodes)item["weather"].First.Value<int>("id"),
                             item.Value<long>("dt").ToDateTimeFromUnixEpoch()));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
             

            return AverageForecastsIntoDailyWindows(forecasts); 
        }

        private IEnumerable<Forecast> AverageForecastsIntoDailyWindows(List<Forecast> forecasts)
        {
            var groups = forecasts.GroupBy(f => f.ForecastDate.Date);

            return groups.Select(grp => CalculateDailyForecast(grp));
        }

        private Forecast CalculateDailyForecast(IGrouping<DateTime, Forecast> grp)
        {
            double temperature = 0;
            string description = null;
            string icon = null;
            WeatherCodes code = default(WeatherCodes);
            DateTime forecastDate = default(DateTime);
            
            temperature = grp.Average(g => g.Temperature);
            description = grp.First().WeatherDescription;
            icon = grp.First().WeatherIconUrl;
            code = grp.First().WeatherCode;
            forecastDate = grp.Key.Date;

            var forecast = new Forecast(grp.First().Location, temperature, description, icon, code, forecastDate);

            return forecast;
        }
    }
}

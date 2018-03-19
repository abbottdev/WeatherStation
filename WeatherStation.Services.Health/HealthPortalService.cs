using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Forecasts;
using WeatherStation.Core.Health;
using WeatherStation.Core.Services;

namespace WeatherStation.Services.Health
{
    public class HealthPortalService : IHealthService
    {
        private string baseAddress;

        public HealthPortalService(string healthPortalAddress)
        {
            this.baseAddress = healthPortalAddress;
        }

        public async Task<ConditionSynopsis> GetConditionDetailsAsync(string conditionId)
        {
            ConditionSynopsis condition;
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(this.baseAddress);

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync($"/conditions/{conditionId}/details");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            JObject result = JsonConvert.DeserializeObject<JObject>(json);

            string name = null;
            string id = null;
            IEnumerable<Condition> symptoms = null;
            IEnumerable<string> complications = null;
            IEnumerable<string> suggestions = null;

            name = result.Value<string>("ConditionName");
            complications = result["Complications"].Values<string>();
            symptoms = result["Symptoms"].Select(token => new Condition(token.Value<string>("Name"), token.Value<string>("Id")));
            suggestions = Enumerable.Empty<string>();

            condition = new ConditionSynopsis(name, id, symptoms, complications, suggestions);

            return condition;
        }

        public async Task<IEnumerable<Condition>> GetConditionsAffectedByWeatherAsync(WeatherCodes weather, double temperature)
        {
            List<Condition> conditions = new List<Condition>();
            string mappedWeatherCondition = "";

            if (temperature < 278.15)
            {
                // < 5 degrees we will class as cold.
                mappedWeatherCondition = "cold";
            }
            else
            {
                //Map weather codes to well-known weather conditions in our API - consider that the API may not know about as in-depth weather as the client..
                switch (weather)
                {
                    case WeatherCodes.snow:
                    case WeatherCodes.shower_rain:
                    case WeatherCodes.light_rain:
                        mappedWeatherCondition = "cold";
                        break;
                    default:
                        mappedWeatherCondition = "";
                        break;
                }
            }

            if (string.IsNullOrEmpty(mappedWeatherCondition)) return Enumerable.Empty<Condition>();

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(this.baseAddress);

            var response = await client.PostAsync($"/conditions/triggered-by/weather/{mappedWeatherCondition}?temperature={temperature}", new StringContent(""));

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<Condition>();
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            //[{"Condition":"Common Cold","Suggestions":["Reduce phsyical activity"]},{"Condition":"Rhinorrhea","Suggestions":["Consider taking decongestants"]},{"Condition":"Asthma","Suggestions":["Increase dosage","Reduce phsyical activity"]}]
            JArray result = JsonConvert.DeserializeObject<JArray>(json);

            foreach (var condition in result)
            {
                conditions.Add(new Condition(condition.Value<string>("Condition"), condition.Value<string>("Id")));
            }

            return conditions;
        }
    }
}

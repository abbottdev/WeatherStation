using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Forecasts;
using WeatherStation.Core.Health;

namespace WeatherStation.Core.Services
{
    public interface IHealthService
    {
        Task<IEnumerable<Condition>> GetConditionsAffectedByWeatherAsync(WeatherCodes weather, double temperature);

        Task<ConditionSynopsis> GetConditionDetailsAsync(string conditionId);
    }
}

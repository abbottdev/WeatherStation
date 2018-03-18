using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Locations;
using WeatherStation.Core.Forecasts;

namespace WeatherStation.Core.Services
{
    public interface IWeatherService
    {

        /// <summary>
        /// Returns the weather conditions for today.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task<TodaysForecast> GetTodaysWeatherAsync(Location location);


        /// <summary>
        /// Returns hourly weather data for a given date and location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task<IEnumerable<Forecast>> GetHourlyWeatherForDateAsync(Location location, DateTime start, TimeSpan duration);


        /// <summary>
        /// Returns the weather forecast
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task<IEnumerable<Forecast>> GetWeatherForecastAsync(Location location);
    }
}

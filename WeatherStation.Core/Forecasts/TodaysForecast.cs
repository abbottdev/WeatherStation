using System;
using System.Collections.Generic;
using System.Text;
using WeatherStation.Core.Locations;

namespace WeatherStation.Core.Forecasts
{

    public class TodaysForecast : Forecast
    {
        public TodaysForecast(Location location, double temperature, string weatherDescription, string weatherIconUrl, WeatherCodes code, DateTime forecastDate, int humidity, int pressure, int windSpeed, DateTime sunrise, DateTime sunset) : 
            base(location, temperature, weatherDescription, weatherIconUrl, code, forecastDate)
        {
            this.Humidity = humidity;
            this.Pressure = pressure;
            this.Sunrise = sunrise;
            this.Sunset = sunset;
            this.WindSpeed = windSpeed;
        }

        public int Humidity { get; set; }
        public int Pressure { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public int WindSpeed { get; set; }
    }
}

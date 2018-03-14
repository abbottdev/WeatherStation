using System;
using System.Collections.Generic;
using System.Text;
using WeatherStation.Core.Locations;

namespace WeatherStation.Core.Forecasts
{

    public class Forecast
    {
        public Locations.Location Location { get; }
        
        public double Temperature { get; }

        public DateTime ForecastDate { get;  }

        public Forecast(Location location, double temperature, DateTime forecastDate)
        {
            this.Temperature = temperature;
            this.ForecastDate = forecastDate;
            this.Location = location;
        }


    }
}

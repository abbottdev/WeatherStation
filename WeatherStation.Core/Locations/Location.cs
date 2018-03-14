using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStation.Core.Locations
{
    public class Location
    {
        public string DisplayName { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public Location(double latitude, double longitude, string displayName)
        {
            this.DisplayName = displayName;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}

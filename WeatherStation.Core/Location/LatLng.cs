using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStation.Core.Locations
{
    public class Location
    {
        private readonly string DisplayName;
        public readonly float Latitude;
        public readonly float Longitude;

        public Location(float latitude, float longitude, string displayName)
        {
            this.DisplayName = displayName;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}

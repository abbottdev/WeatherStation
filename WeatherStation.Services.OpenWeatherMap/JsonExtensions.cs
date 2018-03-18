using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStation
{
    public static class JsonExtensions
    {

        public static DateTime ToDateTimeFromUnixEpoch(this double epoch)
        {
            var dt = new DateTime(1970, 1, 1);

            return dt.AddSeconds(epoch);
        }

        public static double ToUnixEpoch(this DateTime date)
        {
            var dt = new DateTime(1970, 1, 1);

            return (date - dt).TotalSeconds;
        }
        
        public static DateTime ToDateTimeFromUnixEpoch(this long epoch)
        {
            return ((double)epoch).ToDateTimeFromUnixEpoch();
        }
    }
}

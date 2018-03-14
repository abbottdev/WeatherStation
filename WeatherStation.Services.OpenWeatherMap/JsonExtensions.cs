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
        public static DateTime ToDateTimeFromUnixEpoch(this long epoch)
        {
            return ((double)epoch).ToDateTimeFromUnixEpoch();
        }
    }
}

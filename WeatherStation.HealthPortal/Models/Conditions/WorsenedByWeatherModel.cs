using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherStation.HealthPortal.Models.Conditions
{
    public class ComplicatedByWeatherModel
    {
        public IEnumerable<ConditionComplication> Complications { get; set; }

        public string Weather { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherStation.HealthPortal.Models.Conditions
{
    public class ConditionDetails
    {
        public string ConditionName { get; set; }

        public IEnumerable<string> Complications { get; set; }

        public IEnumerable<Symptom> Symptoms { get; set; }
    }
}
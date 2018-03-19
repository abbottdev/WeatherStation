using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherStation.HealthPortal.Models
{
    public class WorsenedCondition
    {
        public WorsenedCondition(string condition, IEnumerable<string> suggestions)
        {
            this.Condition = condition;
            this.Suggestions = suggestions;
        }

        public string Condition { get; }
        public IEnumerable<string> Suggestions { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherStation.HealthPortal.Models.Conditions
{
    public class ConditionComplication
    {
        public ConditionComplication(string conditionId, string conditionName, IEnumerable<string> suggestions)
        {
            this.Id = conditionId;
            this.Condition = conditionName;
            this.Suggestions = suggestions;
        }

        public string Id { get; }

        public string Condition { get; }
        public IEnumerable<string> Suggestions { get; }
    }
}
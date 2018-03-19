﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStation.Core.Health
{
    public class ConditionSynopsis : Condition
    {
        public ConditionSynopsis(string name, string id, IEnumerable<string> symptoms, IEnumerable<string> complications, IEnumerable<string> suggestions) : base(name, id)
        {
            this.Symptoms = symptoms;
            this.Complications = complications;
            this.Suggestions = suggestions;
        }

        public IEnumerable<string> Symptoms { get; }
        public IEnumerable<string> Complications { get; }
        public IEnumerable<string> Suggestions { get; }

    }
}

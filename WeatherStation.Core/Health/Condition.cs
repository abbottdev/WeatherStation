using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherStation.Core.Health
{
    public class Condition
    {

        public Condition(string name, string id)
        {
            this.Id = id;
            this.Name = name;

        }
         
        public string Id { get; }
        public string Name { get; }
    }
}

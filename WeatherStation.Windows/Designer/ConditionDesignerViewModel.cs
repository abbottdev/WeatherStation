using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Health;
using WeatherStation.Core.Services;
using WeatherStation.Windows.ViewModels;

namespace WeatherStation.Windows.Designer
{
    public class ConditionDesignerViewModel : ConditionViewModel
    {
        public ConditionDesignerViewModel() : base(null, "", "Asthma", null)
        {
        }

        public override IEnumerable<string> Complications => new string[] { "Common Cold", "Cold Weather" };

        public override IEnumerable<ConditionViewModel> Symptoms => new ConditionViewModel[] { new ConditionViewModel(null, "", "Shortness of breath", null) };

        public override IEnumerable<string> Suggestions => new string[] { "Increase Medication", "Avoid physical activity" };
    }
}

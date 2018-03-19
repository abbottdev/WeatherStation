using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Core.Services;

namespace WeatherStation.Windows.ViewModels
{
    public class ConditionViewModel : ReactiveObject, IRoutableViewModel
    {
        private ObservableAsPropertyHelper<IEnumerable<string>> complicationsProperty;
        private ObservableAsPropertyHelper<IEnumerable<string>> symptomsProperty;
        private ObservableAsPropertyHelper<IEnumerable<string>> suggestionsProperty;

        public string ConditionName { get; }


        public ConditionViewModel(AppViewModel screen, string conditionId, string conditionName, IHealthService service)
        {
            this.HostScreen = screen;
            this.ConditionName = conditionName;

            var loadConditionFromApi = ReactiveCommand.CreateFromTask(async () =>
            {
                var details = await service.GetConditionDetailsAsync(conditionId);
                return details;
            });

            loadConditionFromApi.ThrownExceptions.Subscribe(e =>
            {
                Debug.WriteLine(e.Message);
            });

            var conditionDetails = loadConditionFromApi.Publish().RefCount();

            this.complicationsProperty = conditionDetails.Select(c => c.Complications).ToProperty(this, vm => vm.Complications);
            this.symptomsProperty = conditionDetails.Select(c => c.Symptoms).ToProperty(this, vm => vm.Symptoms);
            this.suggestionsProperty = conditionDetails.Select(c => c.Suggestions).ToProperty(this, vm => vm.Suggestions);

            this.WhenNavigatedTo(() => Task.Run(async () => await loadConditionFromApi.Execute()));

            this.ViewCommand = ReactiveCommand.Create(() => screen.Router.Navigate.Execute(this).Subscribe());
        }

        public string UrlPathSegment => "conditions/" + this.ConditionName;

        public IScreen HostScreen { get; }

        public IEnumerable<string> Complications => this.complicationsProperty.Value;

        public IEnumerable<string> Symptoms => this.symptomsProperty.Value;

        public IEnumerable<string> Suggestions => this.suggestionsProperty.Value;

        public ReactiveCommand<Unit, IDisposable> ViewCommand { get; }
    }
}

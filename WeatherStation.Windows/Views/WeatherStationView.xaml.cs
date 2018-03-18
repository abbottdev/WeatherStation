using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeatherStation.Windows.ViewModels;

namespace WeatherStation.Windows.Views
{
    /// <summary>
    /// Interaction logic for ForecastView.xaml
    /// </summary>
    public partial class WeatherStationView : UserControl, IViewFor<WeatherStationViewModel>
    {

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(WeatherStationViewModel), typeof(WeatherStationView), new PropertyMetadata(default(WeatherStationViewModel)));

        public WeatherStationViewModel ViewModel
        {
            get { return (WeatherStationViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (WeatherStationViewModel)value; }
        }

        public WeatherStationView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }
    }
}

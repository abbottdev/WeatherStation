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
    /// Interaction logic for ConditionView.xaml
    /// </summary>
    public partial class ConditionView : UserControl, IViewFor<ConditionViewModel>
    {

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ConditionViewModel), typeof(ConditionView), new PropertyMetadata(default(ConditionViewModel)));

        public ConditionViewModel ViewModel
        {
            get { return (ConditionViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ConditionViewModel)value; }
        }


        public ConditionView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }
    }
}

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

namespace WeatherStation.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public AppViewModel AppViewModel
        {
            get { return (AppViewModel)GetValue(AppViewModelProperty); }
            set { SetValue(AppViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppViewModelProperty =
            DependencyProperty.Register("AppViewModel", typeof(AppViewModel), typeof(MainWindow));


        public MainWindow()
        {
            InitializeComponent();

            var bootstrapper = new AppViewModel();

            this.AppViewModel = bootstrapper;
        }
    }
}

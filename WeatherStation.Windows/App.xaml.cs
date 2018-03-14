using Akavache;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeatherStation.Windows.ViewModels;

namespace WeatherStation.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            BlobCache.ApplicationName = "WeatherStation.Wpf";
        }
    }
}

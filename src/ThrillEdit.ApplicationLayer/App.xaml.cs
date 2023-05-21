using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ThrillEdit.ApplicationLayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        App()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, service) => {


                    service.AddSingleton<MainWindow>
                    (
                    );
                })
                .Build();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}

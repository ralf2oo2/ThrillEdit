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
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Providers;

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
                    service.AddSingleton<VorbisEdit>();
                    service.AddSingleton<ItemProvider>();
                    service.AddSingleton
                    (
                         (services) => new MainWindow
                             (
                                 services.GetRequiredService<VorbisEdit>(),
                                 services.GetRequiredService<ItemProvider>()
                             )
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

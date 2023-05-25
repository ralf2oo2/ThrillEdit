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
using ThrillEdit.BusinessLayer;
using ThrillEdit.BusinessLayer.Providers;
using ThrillEdit.BusinessLayer.Models;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace ThrillEdit.ApplicationLayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly IHost _host;
        App()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, service) => {
                    service.AddSingleton<VorbisEdit>();
                    service.AddSingleton<ItemProvider>();
                    service.AddSingleton<ProgressBar>();
                    service.AddSingleton<ApplicationSettings>();
                    service.AddSingleton
                    (
                         (services) => new MainWindow
                             (
                                 services.GetRequiredService<ViewModelSelector>(),
                                 services.GetRequiredService<ItemProvider>(),
                                 services.GetRequiredService<ProgressBar>(),
                                 services.GetRequiredService<ApplicationSettings>()
                             )
                    );
                    service.AddSingleton
                    (
                         (services) => new ViewModelSelector
                             (
                                 services.GetRequiredService<VorbisEdit>(),
                                 services.GetRequiredService<ProgressBar>()
                             )
                    );
                })
                .Build();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationSettings applicationSettings = _host.Services.GetRequiredService<ApplicationSettings>();
            Settings settings = applicationSettings.GetApplicationSettings();
            while(settings.GameDirectory == null)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select ThrillVille root directory";
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        string[] files = Directory.GetFiles(fbd.SelectedPath);

                        if(files.Where(x => Path.GetFileName(x) == "Thrillville07.exe") != null)
                        {
                            settings.GameDirectory = fbd.SelectedPath;
                            applicationSettings.SaveApplicationSettings(settings);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("'Thrillville07.exe' not found, Are you sure this is the right directory?");
                        }
                    }
                    if(result == DialogResult.Cancel)
                    {
                        Environment.Exit(0);
                    }
                }
            }
            MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}

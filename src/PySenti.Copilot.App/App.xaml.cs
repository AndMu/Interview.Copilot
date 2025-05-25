using Microsoft.Extensions.DependencyInjection;
using PySenti.Copilot.App.ViewModels;
using PySenti.Copilot.Definitions;
using PySenti.Copilot.Windows.Audio;
using PySenti.Copilot.Windows.Screens;
using System.Windows;
using PySenti.Copilot.Azure;
using PySenti.Copilot.App.Helpers;
using Microsoft.IO;
using PySenti.Copilot.App.Views;

namespace PySenti.Copilot.App
{
    public partial class App 
    {
        private ServiceProvider? serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);           

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

            // Get the view model from the service provider
            var window = serviceProvider.GetRequiredService<MainWindow>();
            window.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddAudio();
            services.AddAzure();
            services.AddCopilot();
            services.AddScoped<MainWindow>();
            services.AddSingleton<IStatePersistence, StatePersistence>();
            services.AddSingleton<RecyclableMemoryStreamManager>();
            services.AddSingleton<IScreenShot, ScreenShot>();
            services.AddSingleton<IImageHelper, ImageHelper>();
            services.AddSingleton<MainWindowViewModel>();
        }
    }
}
using MAVPC.MVVM.ViewModels;
using MAVPC.MVVM.Views;
using MAVPC.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MAVPC
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();

            // HTTP Client
            services.AddHttpClient();

            // Servicios
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ITrafficService, TrafficService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<MapViewModel>();
            services.AddTransient<StatsViewModel>();

            // Vistas
            services.AddSingleton<MainView>(provider => new MainView
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainView>();
            mainWindow.Show();
        }
    }
}
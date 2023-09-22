using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.UI.ViewModels;

namespace MO2AutoPacker.UI
{
    public partial class App : Application
    {
        public new static App Current { get; private set; } = null!;
        public IServiceProvider ServiceProvider { get; }
        
        public App()
        {
            var services = new ServiceCollection();
            AddServices(services);
            ServiceProvider = services.BuildServiceProvider();
            Current = this;
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IMessenger>(new WeakReferenceMessenger());
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<BannerViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow(
                ServiceProvider.GetService<MainWindowViewModel>()!);
            MainWindow.Show();
        }
    }
}
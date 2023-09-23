using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using MO2AutoPacker.Library;
using MO2AutoPacker.Library.ViewModels;
using MO2AutoPacker.UI.Implementations;

namespace MO2AutoPacker.UI
{
    public partial class App : Application
    {
        public App()
        {
            Services.Initialize(new WpfDispatcher());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow(Services.Provider.GetService<MainWindowViewModel>()!);
            MainWindow.Show();
        }
    }
}
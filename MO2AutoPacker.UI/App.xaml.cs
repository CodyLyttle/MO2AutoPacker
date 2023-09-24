using System.Windows;
using MO2AutoPacker.Library.ViewModels;
using MO2AutoPacker.UI.Implementations;
using MO2AutoPacker.UI.Views;
using ServiceProvider = MO2AutoPacker.Library.ServiceProvider;

namespace MO2AutoPacker.UI
{
    public partial class App : Application
    {
        public App()
        {
            ServiceProvider.Initialize(new WpfDispatcher());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow(ViewModelProvider.GetViewModel<MainWindowViewModel>());
            MainWindow.Show();
        }
    }
}
using System;
using System.Windows;
using MO2AutoPacker.Library;
using MO2AutoPacker.UI.Implementations;

namespace MO2AutoPacker.UI;

public partial class App : Application
{
    public App()
    {
        Services = Bootstrapper.CreateServiceProvider(new WpfDispatcher(), new WindowsPathPicker());
        InitializeComponent();
    }

    public new static App Current => (App) Application.Current;

    public IServiceProvider Services { get; }
}
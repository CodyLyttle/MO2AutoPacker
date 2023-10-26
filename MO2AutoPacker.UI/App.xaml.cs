using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using MO2AutoPacker.Library;
using MO2AutoPacker.UI.Implementations;

namespace MO2AutoPacker.UI;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        Services = Bootstrapper.Bootstrap(new WindowsConfirmationDialog(),
            new WpfDispatcher(),
            new WindowsPathPicker());

        InitializeComponent();
    }

    public new static App Current => (App) Application.Current;

    public IServiceProvider Services { get; }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        using TextWriter writer = File.AppendText("crash.txt");
        writer.WriteLine(DateTime.Now);
        writer.WriteLine(e.Exception.Message);
        writer.WriteLine();
        writer.Close();

        MessageBox.Show(e.Exception.Message, "Unrecoverable error", MessageBoxButton.OK, MessageBoxImage.Error);
        MainWindow!.Close();
    }
}
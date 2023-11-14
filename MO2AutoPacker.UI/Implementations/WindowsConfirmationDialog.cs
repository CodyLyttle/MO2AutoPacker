using System.Windows;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.UI.Implementations;

public class WindowsConfirmationDialog : IConfirmationDialog
{
    private readonly IUIThreadDispatcher _dispatcher;

    public WindowsConfirmationDialog(IUIThreadDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public bool PromptUser(string title, string description) =>
        _dispatcher.Invoke(() => MessageBox.Show(App.Current.MainWindow!, description, title, MessageBoxButton.YesNo)
            is MessageBoxResult.Yes);
}
using System;
using System.Windows;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.UI.Controls;

namespace MO2AutoPacker.UI.Implementations;

public class WpfConfirmationDialog : IConfirmationDialog
{
    private readonly IUIThreadDispatcher _dispatcher;

    public WpfConfirmationDialog(IUIThreadDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public bool PromptUser(string title, string description)
    {
        return _dispatcher.Invoke(() =>
        {
            Window? owner = App.Current.MainWindow;
            if (owner == null)
            {
                throw new InvalidOperationException("Cannot create a confirmation dialog without a main window");
            }

            ConfirmationDialog dialogWindow = new(title, description)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            
            bool? result = dialogWindow.ShowDialog();
            return result.HasValue && result.Value;
        });
    }
}
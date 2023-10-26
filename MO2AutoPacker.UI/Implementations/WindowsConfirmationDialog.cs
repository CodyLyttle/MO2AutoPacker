using System.Windows;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.UI.Implementations;

public class WindowsConfirmationDialog : IConfirmationDialog
{
    public bool PromptUser(string title, string caption) =>
        MessageBox.Show(title, caption, MessageBoxButton.YesNo) is MessageBoxResult.Yes;
}
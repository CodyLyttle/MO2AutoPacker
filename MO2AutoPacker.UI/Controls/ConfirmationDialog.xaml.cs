using System.Windows;
using System.Windows.Input;

namespace MO2AutoPacker.UI.Controls;

public partial class ConfirmationDialog : Window
{
    public ConfirmationDialog(string title, string description)
    {
        InitializeComponent();

        Title = title;
        Description = description;

        CancelButton.Click += (_, _) => DialogResult = false;
        NoButton.Click += (_, _) => DialogResult = false;
        YesButton.Click += (_, _) => DialogResult = true;
    }

    public string Description { get; }

    private void Border_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}
using System.Windows;
using System.Windows.Forms;
using MO2AutoPacker.UI.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MO2AutoPacker.UI.CustomControls;

public partial class PathPicker : UserControl
{
    private PathPickerViewModel? _viewmodel;

    public PathPicker()
    {
        InitializeComponent();
        DataContextChanged += (_, e) =>
        {
            if (e.NewValue is PathPickerViewModel vm)
            {
                _viewmodel = vm;
            }
        };
    }

    private void BtnOpenDialog_OnClick(object sender, RoutedEventArgs e)
    {
        using FolderBrowserDialog dialog = new();
        DialogResult result = dialog.ShowDialog();

        string? pickedPath = result == DialogResult.OK
            ? dialog.SelectedPath
            : null;

        _viewmodel!.UpdatePathCommand.Execute(pickedPath);
    }
}
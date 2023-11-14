using CommunityToolkit.Mvvm.ComponentModel;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isEnabled = true;
}
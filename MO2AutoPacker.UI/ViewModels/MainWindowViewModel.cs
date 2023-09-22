using CommunityToolkit.Mvvm.Messaging;

namespace MO2AutoPacker.UI.ViewModels;

// The primary class responsible for managing application state.
// Any dependencies should be configured in App.xaml.cs and injected here.
public class MainWindowViewModel : ViewModelBase
{
    public PathPickerViewModel RootPathPicker { get; }

    public MainWindowViewModel(IMessenger messenger)
    {
        RootPathPicker = new PathPickerViewModel(messenger, "Mod Organizer 2 path");
    }
}
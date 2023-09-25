using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;

namespace MO2AutoPacker.Library.ViewModels;

// The primary class responsible for managing application state.
// Any dependencies should be configured in App.xaml.cs and injected here.
public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(IMessenger messenger)
    {
        RootPathPicker = new PathPickerViewModel(messenger, PathKey.ModOrganizerRoot, "Mod Organizer 2 path");
        ProfileSelector = new ProfileSelectorViewModel(messenger);
    }

    public PathPickerViewModel RootPathPicker { get; }
    public ProfileSelectorViewModel ProfileSelector { get; }
}
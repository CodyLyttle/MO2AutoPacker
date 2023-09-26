using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;
    private readonly IPathPicker _pathPicker;

    [ObservableProperty]
    private string _modOrganizerPath = string.Empty;

    public MainWindowViewModel(IMessenger messenger, IPathPicker pathPicker)
    {
        _messenger = messenger;
        _pathPicker = pathPicker;
    }

    [RelayCommand]
    private void PickModOrganizerPath()
    {
        DirectoryInfo? newDir = _pathPicker.PickDirectory();
        ModOrganizerPath = newDir is null
            ? string.Empty
            : newDir.FullName;

        _messenger.Send(new PathChangedMessage(PathKey.ModOrganizerRoot, ModOrganizerPath));
    }
}
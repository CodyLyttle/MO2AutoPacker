using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDirectoryManager _directoryManager;
    private readonly IMessenger _messenger;
    private readonly IPathPicker _pathPicker;

    [ObservableProperty]
    private string _modOrganizerPath = string.Empty;

    public MainWindowViewModel(IMessenger messenger, IPathPicker pathPicker, IDirectoryManager directoryManager)
    {
        _messenger = messenger;
        _pathPicker = pathPicker;
        _directoryManager = directoryManager;
    }

    [RelayCommand]
    private void PickModOrganizerPath()
    {
        DirectoryInfo? newDir = _pathPicker.PickDirectory();

        // Picker cancelled/closed or identical path picked.
        if (newDir == null || newDir.FullName == ModOrganizerPath)
            return;

        try
        {
            _directoryManager.SetModOrganizerFolder(newDir.FullName);
            ModOrganizerPath = newDir.FullName;
        }
        catch (DirectoryNotFoundException ex)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, ex.Message));
            return;
        }

        _messenger.Send(new ModOrganizerPathChangedMessage());
    }
}
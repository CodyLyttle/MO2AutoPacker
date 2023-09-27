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
    private readonly IDirectoryManager _directoryManager;

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
        if (newDir == null) // Closed/cancelled path picker.
            return;

        try
        {
            _directoryManager.SetModOrganizerFolder(newDir.FullName);
        }
        catch (DirectoryNotFoundException ex)
        {
            ModOrganizerPath = string.Empty;
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, ex.Message));
            return;
        }

        _messenger.Send(new ModOrganizerPathChanged());
        throw new Exception("Shit's fucked yo");
    }
}
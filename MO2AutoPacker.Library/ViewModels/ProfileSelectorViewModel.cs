using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.ViewModels;

// TODO: Respond to file system changes such as profile added/removed.
public partial class ProfileSelectorViewModel : ViewModelBase, IRecipient<PathChangedMessage>
{   
    private readonly IMessenger _messenger;
    private string? _profilesPath;

    [ObservableProperty]
    private List<Profile> _profiles = new();

    private Profile? _selectedProfile;
    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (value == _selectedProfile)
                return;

            _selectedProfile = value;
            _messenger.Send(new ProfileChangedMessage(value));
        }
    }

    public string[] ProfileNames => ReadProfileNames(_profilesPath);
    
    public ProfileSelectorViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register(this);        
    }
    
    public void Receive(PathChangedMessage message)
    {
        if (message.Key != PathKey.ModOrganizerRoot)
            return;

        // Message sender has already verified 'profiles' exists.
        _profilesPath = Path.Combine(message.Path, "profiles");
        
        SelectedProfile = null;
        Profiles = Directory.EnumerateDirectories(_profilesPath)
            .Select(Path.GetFileName)
            .Select(fn => new Profile(_profilesPath, fn!))
            .ToList();
        
        OnPropertyChanged(nameof(Profiles));
    }

    private static string[] ReadProfileNames(string? path)
    {
        if (path == null)
            return Array.Empty<string>();

        return Directory.EnumerateDirectories(path)
            .Select(Path.GetFileName)
            .ToArray()!;
    }
}
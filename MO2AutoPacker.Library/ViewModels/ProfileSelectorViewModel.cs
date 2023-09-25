using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ProfileSelectorViewModel : ViewModelBase, IRecipient<PathChangedMessage>
{
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private Profile[] _profiles = Array.Empty<Profile>();

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

    public IEnumerable<string> ProfileNames => Profiles.Select(p => p.Name);

    public ProfileSelectorViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register(this);
    }

    public void Receive(PathChangedMessage message)
    {
        if (message.Key != PathKey.ModOrganizerRoot)
            return;

        // Clear profiles early to ensure the UI is wiped in the event of an error.
        Profiles = Array.Empty<Profile>();
        SelectedProfile = null;

        string profilesPath = Path.Combine(message.Path, "profiles");
        DirectoryInfo profilesDir = new DirectoryInfo(profilesPath);
        if (!profilesDir.Exists)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error,
                "Invalid MO2 directory - missing subdirectory 'profiles'"));
        }
        else
        {
            Profiles = profilesDir.EnumerateDirectories()
                .Select(dir => new Profile(dir))
                .ToArray();
        }
        
        // Update the UI regardless of profile read outcome.
        OnPropertyChanged(nameof(Profiles));
    }
}
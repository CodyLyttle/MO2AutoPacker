using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ProfileSelectorViewModel : ViewModelBase, IRecipient<ModOrganizerPathChanged>
{
    private readonly IMessenger _messenger;
    private readonly IDirectoryReader _reader;


    public ProfileSelectorViewModel(IMessenger messenger, IDirectoryReader reader)
    {
        _messenger = messenger;
        _messenger.Register(this);
        _reader = reader;
    }

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

    public void Receive(ModOrganizerPathChanged message)
    {
        SelectedProfile = null;
        Profiles = _reader.GetProfileFolders()
            .Select(dir => new Profile(dir))
            .ToArray();
    }
}
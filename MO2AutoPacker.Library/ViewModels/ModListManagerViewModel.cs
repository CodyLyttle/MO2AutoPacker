using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ModListManagerViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    private const string ModsFolderName = "mods";
    private const string ModListFileName = "modlist.txt";
    private readonly IDirectoryReader _directoryReader;

    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ModList? _modList;

    public ModListManagerViewModel(IMessenger messenger, IDirectoryReader directoryReader)
    {
        _messenger = messenger;
        _messenger.Register(this);
        _directoryReader = directoryReader;
    }

    public void Receive(ProfileChangedMessage message)
    {
        // Clear mods early to ensure UI is wiped in the event of an error.
        ModList = null;

        if (message.Profile is null)
            return;

        string modListFilePath = Path.Combine(message.Profile.Directory.FullName, ModListFileName);
        FileInfo modListFile = new(modListFilePath);
        if (!modListFile.Exists)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error,
                $"Invalid MO2 profile - missing file '{ModListFileName}'"));
            return;
        }

        List<IModListItem> items = new();
        using TextReader reader = modListFile.OpenText();
        string? line = reader.ReadLine();
        while (line != null)
        {
            IModListItem? item = ParseModFromManifestLine(line);
            if (item != null)
                items.Add(item);

            line = reader.ReadLine();
        }

        // Order low->high priority.
        items.Reverse();
        ModList = new ModList(message.Profile.Name, items.ToArray());
    }

    // TODO: Extract ModListReader service.
    private IModListItem? ParseModFromManifestLine(string line)
    {
        if (line.Length == 0)
            return null;

        char prefix = line[0];
        string name = line[1..];

        // Line represents something other than a mod or separator, such as official DLC or a comment.
        if (prefix is not '-' && prefix is not '+')
            return null;

        if (line.EndsWith("_separator"))
            return new ModSeparator(name[..^10]); // Trim suffix.

        return new Mod(name, prefix is '+');
    }

    [RelayCommand]
    private void PackFiles()
    {
        if (ModList is null)
            throw new InvalidOperationException("Cannot pack an empty mod list");

        _messenger.Send(new PackRequestMessage(ModList));
    }
}
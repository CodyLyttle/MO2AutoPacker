using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ModListManagerViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    private const string ModsFolderName = "mods";
    private const string ModListFileName = "modlist.txt";

    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ModList? _modList;

    public ModListManagerViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register(this);
    }

    public void Receive(ProfileChangedMessage message)
    {
        // Clear mods early to ensure UI is wiped in the event of an error.
        ModList = null;

        if (message.Profile is null)
            return;

        string mo2RootPath = message.Profile.Directory.Parent!.Parent!.FullName;
        string modsPath = Path.Combine(mo2RootPath, ModsFolderName);
        DirectoryInfo modsDir = new(modsPath);
        if (!modsDir.Exists)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error,
                "Invalid MO2 directory - missing subdirectory 'mods'"));
            return;
        }

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
            IModListItem? item = ParseModFromManifestLine(modsDir, line);
            if (item != null)
                items.Add(item);

            line = reader.ReadLine();
        }

        // Order low->high priority.
        items.Reverse();
        ModList = new ModList(message.Profile.Name, items.ToArray());
    }

    private IModListItem? ParseModFromManifestLine(DirectoryInfo modsPath, string line)
    {
        if (line.Length == 0)
            return null;

        if (line.EndsWith("_separator"))
            // Trim prefix '-' and suffix '_separator'
            return new ModSeparator(line[1..^10]);

        char firstChar = line[0];
        if (firstChar is not '-' && firstChar is not '+')
            return null;

        string modName = line[1..];
        string modPath = Path.Combine(modsPath.FullName, modName);
        DirectoryInfo modDirectory = new(modPath);
        return modDirectory.Exists
            ? new Mod(modName, modDirectory, line[0] == '+')
            : null; // TODO: Should we display a warning to the user?
    }

    [RelayCommand]
    private void PackFiles()
    {
        if (ModList is null)
            throw new InvalidOperationException("Cannot pack an empty mod list");

        _messenger.Send(new PackRequestMessage(ModList));
    }
}
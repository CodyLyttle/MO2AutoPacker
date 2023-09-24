using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ModListManagerViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
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
        const string modListFileName = "modlist.txt";

        if (message.Profile is null)
        {
            ModList = null;
        }
        else
        {
            // PathPicker has already validated 'mods' path.
            string modsPath = Path.Combine(message.Profile.RootPath, "mods");
            
            string modListPath = Path.Combine(message.Profile.Path, modListFileName);
            if (File.Exists(modListPath))
            {
                _messenger.Send(new BannerMessage(BannerMessage.Type.Error,
                    $"The selected profile is missing '{modListFileName}'"));

                return;
            }
            
            using TextReader reader = File.OpenText(modListPath);
            List<IModListItem> items = new();
            string? line = reader.ReadLine();
            while (line != null)
            {
                IModListItem? item = ParseModFromManifestLine(modsPath, line);
                if(item != null)
                    items.Add(item);

                line = reader.ReadLine();
            }

            // Order low->high priority.
            items.Reverse();
            ModList = new ModList(message.Profile.Name, items.ToArray());
        }
    }

    private IModListItem? ParseModFromManifestLine(string modsPath, string line)
    {
        if (line.EndsWith("_separator"))
        {
            // Trim prefix '-' and suffix '_separator'
            return new ModSeparator(line[1..^10]);
        }

        if (!line.StartsWith('-') && !line.StartsWith('+'))
            return null;

        string name = line[1..];
        string path = Path.Combine(modsPath, name);
        return new Mod(name, path, line[0] == '+');
    }

    [RelayCommand]
    private void PackFiles()
    {
        if (ModList is null)
            throw new InvalidOperationException("Cannot pack an empty mod list");

        _messenger.Send(new PackRequestMessage(ModList));
    }
}
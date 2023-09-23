using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ModListManagerViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    [ObservableProperty]
    private ModList? _modList;

    public ModListManagerViewModel(IMessenger messenger)
    {
        messenger.Register(this);
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
            string modListPath = Path.Combine(message.Profile.Path, modListFileName);
            if (!File.Exists(modListPath))
                throw new InvalidOperationException($"The selected profile is missing file '{modListFileName}'");

            List<IModListItem> mods = new();
            using TextReader reader = File.OpenText(modListPath);

            string? line = reader.ReadLine();
            while (line != null)
            {
                // '+' signifies an activated mod.
                if (line.StartsWith('+'))
                {
                    string name = line[1..];
                    mods.Add(new Mod(name, true));
                }
                // '-' signifies a deactivated mod or separator.
                else if (line.StartsWith('-'))
                {
                    string name = line[1..];
                    mods.Add(line.EndsWith("_separator")
                        ? new ModSeparator(name[..^10]) // Remove _separator suffix. 
                        : new Mod(name, false));
                }

                line = reader.ReadLine();
            }

            // Order low->high priority.
            mods.Reverse();
            ModList = new ModList(message.Profile.Name, mods.ToArray());
        }
    }
}
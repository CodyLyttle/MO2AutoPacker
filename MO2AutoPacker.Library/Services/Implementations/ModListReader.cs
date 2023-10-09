using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Services.Implementations;

public class ModListReader : IModListReader
{
    private readonly IDirectoryReader _directoryReader;

    public ModListReader(IDirectoryReader directoryReader)
    {
        _directoryReader = directoryReader;
    }

    public ModList Read(Profile profile)
    {
        FileInfo info = _directoryReader.GetModList(profile.Name);
        using TextReader reader = info.OpenText();
        string? nextLine = reader.ReadLine();
        List<IModListItem> items = new();

        while (nextLine is not null)
        {
            IModListItem? item = ParseModFromManifestLine(nextLine);
            if (item != null)
                items.Add(item);

            nextLine = reader.ReadLine();
        }

        // Order low->high priority.
        items.Reverse();
        return new ModList(profile.Name, items.ToArray());
    }

    private static IModListItem? ParseModFromManifestLine(string line)
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
}
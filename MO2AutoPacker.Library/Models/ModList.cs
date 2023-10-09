namespace MO2AutoPacker.Library.Models;

// TODO: Flesh this class out with Add/Remove, prevent duplicates etc...
public class ModList
{
    private readonly List<IModListItem> _items;

    public ModList(string profileName, params IModListItem[] items)
    {
        ProfileName = profileName;
        _items = items.ToList();
    }

    public string ProfileName { get; }
    public IEnumerable<IModListItem> Items => _items;
    public IEnumerable<ModSeparator> GetSeparators() => _items.OfType<ModSeparator>();
    public IEnumerable<Mod> GetMods() => _items.OfType<Mod>();
    public IEnumerable<Mod> GetModsDisabled() => GetMods().Where(m => !m.IsEnabled);
    public IEnumerable<Mod> GetModsEnabled() => GetMods().Where(m => m.IsEnabled);
}
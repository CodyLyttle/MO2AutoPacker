namespace MO2AutoPacker.Library.Models;

// TODO: Flesh this class out with Add/Remove, prevent duplicates etc...
public class ModList
{
    private readonly List<IModListItem> _items;
    public IEnumerable<IModListItem> Items => _items;
    public string Name { get; }

    public ModList(string name, params IModListItem[] items)
    {
        Name = name;
        _items = items.ToList();
    }
}
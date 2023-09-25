namespace MO2AutoPacker.Library.Models;

public class ModSeparator : IModListItem
{
    private readonly Dictionary<string, Mod> _mods = new();

    public ModSeparator(string name, params Mod[] mods)
    {
        Name = name;
        foreach (Mod mod in mods)
            AddMod(mod);
    }

    public IEnumerable<Mod> Mods => _mods.Values;

    public string Name { get; set; }

    public string ToModListLine() => $"-{Name}_separator";

    public void AddMod(Mod mod)
    {
        if (_mods.ContainsKey(mod.Name))
            throw new InvalidOperationException("Separator already contains the specified mod");

        _mods[mod.Name] = mod;
    }

    public void RemoveMod(Mod mod)
    {
        if (!_mods.Remove(mod.Name))
            throw new InvalidOperationException("Separator does not contain the specified mod");
    }

    public bool ContainsMod(Mod mod) => _mods.ContainsKey(mod.Name);
}
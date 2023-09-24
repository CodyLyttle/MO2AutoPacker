namespace MO2AutoPacker.Library.Models;

public class Mod : IModListItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public bool IsEnabled { get; set; }

    public Mod(string name, string path, bool isEnabled)
    {
        Name = name;
        Path = path;
        IsEnabled = isEnabled;
    }

    public string ToModListLine()
    {
        char prefix = IsEnabled ? '+' : '-';
        return prefix + Name;
    }
}
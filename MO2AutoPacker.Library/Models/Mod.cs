namespace MO2AutoPacker.Library.Models;

public class Mod : IModListItem
{
    public Mod(string name, DirectoryInfo directory, bool isEnabled)
    {
        Name = name;
        Directory = directory;
        IsEnabled = isEnabled;
    }

    public DirectoryInfo Directory { get; set; }
    public bool IsEnabled { get; set; }
    public string Name { get; set; }

    public string ToModListLine()
    {
        char prefix = IsEnabled ? '+' : '-';
        return prefix + Name;
    }
}
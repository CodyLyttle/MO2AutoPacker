namespace MO2AutoPacker.Library.Models;

public class Mod : IModListItem
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }

    public Mod(string name, bool isEnabled)
    {
        Name = name;
        IsEnabled = isEnabled;
    }

    public string ToModListLine()
    {
        char prefix = IsEnabled ? '+' : '-';
        return prefix + Name;
    }
}
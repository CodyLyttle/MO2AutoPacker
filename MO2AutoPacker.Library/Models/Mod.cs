namespace MO2AutoPacker.Library.Models;

public class Mod : IModListItem
{
    public Mod(string name, bool isEnabled)
    {
        Name = name;
        IsEnabled = isEnabled;
    }

    public bool IsEnabled { get; set; }
    public string Name { get; set; }

    public string ToModListLine()
    {
        char prefix = IsEnabled ? '+' : '-';
        return prefix + Name;
    }
}
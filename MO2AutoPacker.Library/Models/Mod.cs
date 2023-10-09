namespace MO2AutoPacker.Library.Models;

public record Mod(string Name, bool IsEnabled) : IModListItem
{
    public bool IsEnabled { get; set; } = IsEnabled;
    public string Name { get; set; } = Name;

    public string ToModListLine()
    {
        char prefix = IsEnabled ? '+' : '-';
        return prefix + Name;
    }
}
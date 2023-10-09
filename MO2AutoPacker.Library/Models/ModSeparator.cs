namespace MO2AutoPacker.Library.Models;

public record ModSeparator(string Name) : IModListItem
{
    public string Name { get; set; } = Name;

    public string ToModListLine() => $"-{Name}_separator";
}
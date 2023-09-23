namespace MO2AutoPacker.Library.Models;

public interface IModListItem
{
    string Name { get; set; }
    string ToModListLine();
}
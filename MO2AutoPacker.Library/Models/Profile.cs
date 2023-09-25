namespace MO2AutoPacker.Library.Models;

public class Profile
{
    public string Name { get; }
    public DirectoryInfo Directory { get; }

    public Profile(DirectoryInfo directory)
    {
        Name = directory.Name;
        Directory = directory;
    }
}
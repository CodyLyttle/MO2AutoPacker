namespace MO2AutoPacker.Library.Models;

public class Profile
{
    public Profile(DirectoryInfo directory)
    {
        Name = directory.Name;
        Directory = directory;
    }

    public string Name { get; }
    public DirectoryInfo Directory { get; }
}
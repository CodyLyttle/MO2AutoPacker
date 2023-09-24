namespace MO2AutoPacker.Library.Models;

public class Profile
{
    public string RootPath { get; }
    public string Name { get; }
    public string Path { get; }

    public Profile(string rootPath, string name)
    {
        RootPath = rootPath;
        Path = System.IO.Path.Combine(rootPath, "profiles", name);
        Name = name;
    }
}
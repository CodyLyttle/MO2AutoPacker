namespace MO2AutoPacker.UI.Models;

public class Profile
{
    public string Name { get; }
    public string Path { get; }

    public Profile(string profilesPath, string profileName)
    {
        Path = System.IO.Path.Combine(profilesPath, profileName);
        Name = profileName;
    }
}
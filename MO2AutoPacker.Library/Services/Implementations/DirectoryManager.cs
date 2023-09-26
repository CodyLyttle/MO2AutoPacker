using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MO2AutoPacker.Library.Tests")]

namespace MO2AutoPacker.Library.Services.Implementations;

internal class DirectoryManager : IDirectoryManager
{
    public const string ModsFolderName = "mods";
    public const string ProfileFolderName = "profiles";
    public const string ModListFileName = "modlist.txt";

    private DirectoryInfo? _modOrganizer;
    private DirectoryInfo? _mods;
    private DirectoryInfo? _profiles;


    public void SetModOrganizer(string path)
    {
        if (!Path.Exists(path))
            throw new DirectoryNotFoundException("Invalid path to MO2 root directory");

        string modsPath = Path.Combine(path, ModsFolderName);
        if (!Path.Exists(modsPath))
            throw new DirectoryNotFoundException("Missing folder in MO2 directory 'mods'");

        string profilesPath = Path.Combine(path, ProfileFolderName);
        if (!Path.Exists(profilesPath))
            throw new DirectoryNotFoundException("Missing folder in MO2 directory 'profiles'");

        _modOrganizer = new DirectoryInfo(path);
        _mods = new DirectoryInfo(modsPath);
        _profiles = new DirectoryInfo(profilesPath);
    }

    public DirectoryInfo GetModOrganizer() => GetDirectory(_modOrganizer);

    public DirectoryInfo GetModsFolder() => GetDirectory(_mods);

    public DirectoryInfo GetModFolder(string modName)
    {
        string modPath = Path.Combine(GetModsFolder().FullName, modName);
        DirectoryInfo modDir = new(modPath);
        if (!modDir.Exists)
            throw new DirectoryNotFoundException($"Missing mod folder '{modName}'");

        return modDir;
    }

    public IEnumerable<DirectoryInfo> GetModFolders() => GetModsFolder().EnumerateDirectories();

    public DirectoryInfo GetProfilesFolder() => GetDirectory(_profiles);

    public IEnumerable<DirectoryInfo> GetProfileFolders() => GetProfilesFolder().EnumerateDirectories();

    private static DirectoryInfo GetDirectory(DirectoryInfo? directory)
    {
        if (directory is null)
            throw new InvalidOperationException("Requested directory is uninitialized");
        if (!directory.Exists)
            throw new DirectoryNotFoundException(directory.FullName);

        return directory;
    }
}
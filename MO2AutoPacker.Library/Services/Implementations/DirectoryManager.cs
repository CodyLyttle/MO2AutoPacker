using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MO2AutoPacker.Library.Tests")]

namespace MO2AutoPacker.Library.Services.Implementations;

internal class DirectoryManager : IDirectoryManager
{
    public const string ArchiverVariableName = "BSARCH";
    public const string ModOrganizerVariableName = "MO2_OBLIVION";
    public const string ModsFolderName = "mods";
    public const string ProfilesFolderName = "profiles";
    public const string ArchiverExecutableName = "BSArch.exe";
    public const string ModListFileName = "modlist.txt";

    private DirectoryInfo? _archiver;
    private FileInfo? _archiverExecutable;

    private DirectoryInfo? _modOrganizer;
    private DirectoryInfo? _mods;
    private DirectoryInfo? _profiles;

    public DirectoryManager(IPathReader pathReader)
    {
        InitializeArchiver(pathReader);
        InitializeModOrganizer(pathReader);
    }

    public bool IsArchiverDirectoryInitialized => _archiver is not null;
    public bool IsModOrganizerDirectoryInitialized => _modOrganizer is not null;

    public void SetModOrganizerFolder(string path)
    {
        if (!Path.Exists(path))
            throw new DirectoryNotFoundException("Invalid path to MO2 root directory");

        string modsPath = Path.Combine(path, ModsFolderName);
        if (!Path.Exists(modsPath))
            throw new DirectoryNotFoundException($"Missing folder in MO2 directory '{ModsFolderName}'");

        string profilesPath = Path.Combine(path, ProfilesFolderName);
        if (!Path.Exists(profilesPath))
            throw new DirectoryNotFoundException($"Missing folder in MO2 directory '{ProfilesFolderName}'");

        _modOrganizer = new DirectoryInfo(path);
        _mods = new DirectoryInfo(modsPath);
        _profiles = new DirectoryInfo(profilesPath);
    }

    public void SetArchiverFolder(string path)
    {
        if (!Path.Exists(path))
            throw new DirectoryNotFoundException("Invalid path to BSArch directory");

        string executablePath = Path.Combine(path, ArchiverExecutableName);
        if (!File.Exists(executablePath))
            throw new FileNotFoundException($"Missing executable file '{ArchiverExecutableName}'");

        _archiver = new DirectoryInfo(path);
        _archiverExecutable = new FileInfo(executablePath);
    }

    public DirectoryInfo GetArchiverFolder() => GetDirectory(_archiver);

    public FileInfo GetArchiverExecutable() => GetFile(_archiverExecutable);

    public DirectoryInfo GetModOrganizerFolder() => GetDirectory(_modOrganizer);

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

    public FileInfo GetModList(string profileName)
    {
        string profilePath = Path.Combine(GetProfilesFolder().FullName, profileName);
        DirectoryInfo profileDir = new(profilePath);
        if (!profileDir.Exists)
            throw new DirectoryNotFoundException($"Missing profile folder '{profileName}'");

        string filePath = Path.Combine(profilePath, ModListFileName);
        FileInfo modListFile = new(filePath);
        if (!modListFile.Exists)
            throw new FileNotFoundException($"Missing file 'modlist.txt' for profile '{profileName}'");

        return modListFile;
    }

    private void InitializeArchiver(IPathReader pathReader)
    {
        _archiverExecutable = pathReader.GetFileFromWorkingDirectory(ArchiverExecutableName)
                              ?? pathReader.GetFileFromEnvironmentVariable(ArchiverVariableName)
                              ?? null;

        _archiver = _archiverExecutable?.Directory;
    }

    private void InitializeModOrganizer(IPathReader pathReader)
    {
        // From working directory.
        DirectoryInfo? modsFolder = pathReader.GetFolderFromWorkingDirectory(ModsFolderName);
        DirectoryInfo? profilesFolder = pathReader.GetFolderFromWorkingDirectory(ProfilesFolderName);
        if (modsFolder is not null && profilesFolder is not null)
        {
            _modOrganizer = pathReader.GetWorkingDirectory();
            _mods = modsFolder;
            _profiles = profilesFolder;
            return;
        }

        // From environment variable.
        DirectoryInfo? modOrganizerFolder = pathReader.GetFolderFromEnvironmentVariable(ModOrganizerVariableName);
        if (modOrganizerFolder is null)
            return;

        string modsFolderPath = Path.Combine(modOrganizerFolder.FullName, ModsFolderName);
        string profilesFolderPath = Path.Combine(modOrganizerFolder.FullName, ProfilesFolderName);
        if (!Directory.Exists(modsFolderPath) || !Directory.Exists(profilesFolderPath))
            return;

        _modOrganizer = modOrganizerFolder;
        _mods = new DirectoryInfo(modsFolderPath);
        _profiles = new DirectoryInfo(profilesFolderPath);
    }

    private static DirectoryInfo GetDirectory(DirectoryInfo? directory)
    {
        if (directory is null)
            throw new InvalidOperationException("Requested directory is uninitialized");
        if (!directory.Exists)
            throw new DirectoryNotFoundException(directory.FullName);

        return directory;
    }

    private static FileInfo GetFile(FileInfo? file)
    {
        if (file is null)
            throw new InvalidOperationException("Requested file is uninitialized");
        if (!file.Exists)
            throw new FileNotFoundException(file.FullName);

        return file;
    }
}
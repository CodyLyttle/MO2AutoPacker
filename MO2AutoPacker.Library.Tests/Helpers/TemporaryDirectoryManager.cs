using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;

namespace MO2AutoPacker.Library.Tests.Helpers;

internal sealed class TemporaryDirectoryManager : IDirectoryManager, IDisposable
{
    private readonly TemporaryFolder _archiver;
    private readonly TemporaryFolder _mods;
    private readonly TemporaryFolder _profiles;
    private readonly TemporaryDirectory _tempDir;

    public TemporaryDirectoryManager()
    {
        _tempDir = new TemporaryDirectory();
        _archiver = _tempDir.Root.AddFolder("Archiver")
            .AddFile(DirectoryManager.ArchiverExecutableName);
        _mods = _tempDir.Root.AddFolder(DirectoryManager.ModsFolderName);
        _profiles = _tempDir.Root.AddFolder(DirectoryManager.ProfilesFolderName);
    }

    public bool IsArchiverDirectoryInitialized => true;

    public bool IsModOrganizerDirectoryInitialized => true;

    public DirectoryInfo GetArchiverFolder() => _archiver.Directory;

    public FileInfo GetArchiverExecutable() => _archiver.Directory.GetFiles()[0];

    public DirectoryInfo GetModOrganizerFolder() => _tempDir.Root.Directory;

    public DirectoryInfo GetModsFolder() => _mods.Directory;

    public DirectoryInfo GetModFolder(string modName)
    {
        DirectoryInfo modDir = new(Path.Combine(GetModsFolder().FullName, modName));
        if (!modDir.Exists)
            throw new DirectoryNotFoundException();

        return modDir;
    }

    public IEnumerable<DirectoryInfo> GetModFolders() => _mods.Directory.EnumerateDirectories();

    public DirectoryInfo GetProfilesFolder() => _profiles.Directory;

    public IEnumerable<DirectoryInfo> GetProfileFolders() => _profiles.Directory.EnumerateDirectories();

    public FileInfo GetModList(string profileName) => throw new NotImplementedException();

    public void SetModOrganizerFolder(string path) => throw new InvalidOperationException("May only be set internally");

    public void SetArchiverFolder(string path) => throw new NotImplementedException();

    public void Dispose() => _tempDir.Dispose();

    public TemporaryFolder AddModFolder() => _mods.AddFolder(GetRandomFolderName());

    public TemporaryFolder AddModFolder(string name) => _mods.AddFolder(name);

    public TemporaryFolder AddProfileFolder() => _profiles.AddFolder(GetRandomFolderName());

    public TemporaryFolder AddProfileFolder(string name) => _profiles.AddFolder(name);

    private static string GetRandomFolderName() => Guid.NewGuid().ToString().Replace('-', ' ');
}
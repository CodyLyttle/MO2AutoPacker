namespace MO2AutoPacker.Library.Services;

public interface IDirectoryReader
{
    bool IsArchiverDirectoryInitialized { get; }
    bool IsModOrganizerDirectoryInitialized { get; }

    DirectoryInfo GetArchiverFolder();
    FileInfo GetArchiverExecutable();
    DirectoryInfo GetModOrganizerFolder();
    DirectoryInfo GetModsFolder();
    DirectoryInfo GetModFolder(string modName);
    IEnumerable<DirectoryInfo> GetModFolders();
    DirectoryInfo GetProfilesFolder();
    IEnumerable<DirectoryInfo> GetProfileFolders();
    FileInfo GetModList(string profileName);
}
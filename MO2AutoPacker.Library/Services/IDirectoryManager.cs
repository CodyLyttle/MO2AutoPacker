namespace MO2AutoPacker.Library.Services;

public interface IDirectoryManager : IDirectoryReader
{
    void SetModOrganizerFolder(string path);
    void SetArchiverFolder(string path);
}
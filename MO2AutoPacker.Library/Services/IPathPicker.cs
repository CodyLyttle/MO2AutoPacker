namespace MO2AutoPacker.Library.Services;

public interface IPathPicker
{
    DirectoryInfo? PickDirectory();
    DirectoryInfo? PickDirectory(DirectoryInfo initialDirectory);

    FileInfo? PickFile(params string[] extensionWhitelist);
    FileInfo? PickFile(DirectoryInfo initialDirectory, params string[] extensionWhitelist);
}
namespace MO2AutoPacker.Library.Services;

public interface IPathReader
{
    DirectoryInfo GetWorkingDirectory();
    DirectoryInfo? GetFolderFromWorkingDirectory(string folderName);
    DirectoryInfo? GetFolderFromEnvironmentVariable(string env);
    FileInfo? GetFileFromWorkingDirectory(string fileName);
    FileInfo? GetFileFromEnvironmentVariable(string env);
}
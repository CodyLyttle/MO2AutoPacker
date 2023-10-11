using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.Tests.Stubs;

public class PathReaderStub : IPathReader
{
    public readonly Dictionary<string, FileInfo> EnvironmentFiles = new();
    public readonly Dictionary<string, DirectoryInfo> EnvironmentFolders = new();
    public readonly Dictionary<string, FileInfo> WorkingDirectoryFiles = new();
    public readonly Dictionary<string, DirectoryInfo> WorkingDirectoryFolders = new();
    public DirectoryInfo WorkingDirectory { get; set; } = new(Directory.GetCurrentDirectory());

    public DirectoryInfo GetWorkingDirectory() => WorkingDirectory;

    public DirectoryInfo? GetFolderFromWorkingDirectory(string folderName)
    {
        WorkingDirectoryFolders.TryGetValue(folderName, out DirectoryInfo? result);
        return result;
    }

    public DirectoryInfo? GetFolderFromEnvironmentVariable(string env)
    {
        EnvironmentFolders.TryGetValue(env, out DirectoryInfo? result);
        return result;
    }

    public FileInfo? GetFileFromWorkingDirectory(string fileName)
    {
        WorkingDirectoryFiles.TryGetValue(fileName, out FileInfo? result);
        return result;
    }

    public FileInfo? GetFileFromEnvironmentVariable(string env)
    {
        EnvironmentFiles.TryGetValue(env, out FileInfo? result);
        return result;
    }
}
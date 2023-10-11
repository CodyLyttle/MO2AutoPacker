namespace MO2AutoPacker.Library.Services.Implementations;

public class PathReader : IPathReader
{
    public DirectoryInfo GetWorkingDirectory() => new(Directory.GetCurrentDirectory());

    public DirectoryInfo? GetFolderFromWorkingDirectory(string folderName) =>
        GetWorkingDirectory().EnumerateDirectories()
            .FirstOrDefault(x => x.Name == folderName);

    public DirectoryInfo? GetFolderFromEnvironmentVariable(string env)
    {
        string? value = GetEnvironmentVariable(env);
        return Directory.Exists(value)
            ? new DirectoryInfo(value)
            : null;
    }

    public FileInfo? GetFileFromWorkingDirectory(string fileName) =>
        GetWorkingDirectory().EnumerateFiles()
            .FirstOrDefault(x => x.Name == fileName);

    public FileInfo? GetFileFromEnvironmentVariable(string env)
    {
        string? value = GetEnvironmentVariable(env);
        return File.Exists(value)
            ? new FileInfo(value)
            : null;
    }

    private static string? GetEnvironmentVariable(string varName) => Environment.GetEnvironmentVariable(varName);
}
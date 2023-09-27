using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.Tests.Stubs;

public class PathPickerStub : IPathPicker
{
    public DirectoryInfo? DirectoryToReturn { get; set; } = null;
    public FileInfo? FileToReturn { get; set; } = null;

    public DirectoryInfo? PickDirectory() => DirectoryToReturn;

    public DirectoryInfo? PickDirectory(DirectoryInfo initialDirectory) => DirectoryToReturn;

    public FileInfo? PickFile(params string[] extensionWhitelist) => FileToReturn;

    public FileInfo? PickFile(DirectoryInfo initialDirectory, params string[] extensionWhitelist) => FileToReturn;
}
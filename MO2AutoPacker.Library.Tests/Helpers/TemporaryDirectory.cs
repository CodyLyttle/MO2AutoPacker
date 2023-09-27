namespace MO2AutoPacker.Library.Tests.Helpers;

public sealed class TemporaryDirectory : IDisposable
{
    private const string BaseName = "XUnitTempDir";
    private static readonly object Lock = new();

    private static bool _isFirstRun = true;
    private static int _nextId;

    public TemporaryDirectory()
    {
        // Cleanup existing TemporaryDirectory that was not properly disposed.

        lock (Lock)
        {
            if (_isFirstRun)
            {
                _isFirstRun = false;
                CleanupLeftoverDirectories();
            }
        }

        string folderName = BaseName + GetNextId();
        DirectoryInfo tempDir = new(Path.GetTempPath());
        DirectoryInfo directory = tempDir.CreateSubdirectory(folderName);
        Root = new TemporaryFolder(directory);
    }

    public TemporaryFolder Root { get; }

    public void Dispose() => Directory.Delete(Root.Directory.FullName, true);

    private static int GetNextId()
    {
        lock (Lock)
        {
            return _nextId++;
        }
    }

    private static void CleanupLeftoverDirectories()
    {
        var tempDir = new DirectoryInfo(Path.GetTempPath());
        foreach (DirectoryInfo subDir in tempDir.EnumerateDirectories())
        {
            if (subDir.Name.StartsWith(BaseName))
                Directory.Delete(subDir.FullName, true);
        }
    }
}
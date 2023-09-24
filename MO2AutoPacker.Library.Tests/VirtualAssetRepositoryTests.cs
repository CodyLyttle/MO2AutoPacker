using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;

namespace MO2AutoPacker.Library.Tests;

public sealed class VirtualAssetRepositoryTests : IDisposable
{
    private readonly VirtualAssetRepository _testTarget = new();
    private readonly TemporaryDirectory _modDir = new();

    public void Dispose() => _modDir.Dispose();

    private static List<string> GetFilePaths(VirtualAssetRepository vfs)
    {
        return vfs.EnumerateFilePaths()
            .Select(pathPair => Path.Combine(pathPair.Value, pathPair.Key))
            .ToList();
    }

    private static Mod CreateMod(TemporaryDirectory modDirectory, bool isEnabled = false)
        => new(Guid.NewGuid().ToString(), modDirectory.Root.Path, isEnabled);

    [Fact]
    public void AddMod_ShouldAddFilePathsFromAssetFolders()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        List<string> filePaths = new();

        foreach (string folderName in VirtualAssetRepository.AssetFolderNames)
        {
            _modDir.Root
                .AddFolder(folderName)
                .AddFile(out string filePath);

            filePaths.Add(filePath);
        }

        // Act
        _testTarget.AddMod(mod);

        // Assert
        // Ensure vfs repository contains the path of each asset file exactly once.
        List<string> fileRepository = GetFilePaths(_testTarget);
        Assert.Equal(filePaths.Count, fileRepository.Count);
        Assert.Empty(filePaths.Except(fileRepository));
    }

    [Fact]
    public void AddMod_ShouldAddFilePathsFromNestedAssetFolders()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFolder("nested")
            .AddFile("file.txt", out string expectedPath);

        // Act
        _testTarget.AddMod(mod);

        // Assert
        List<string> fileRepository = GetFilePaths(_testTarget);
        Assert.Single(fileRepository);
        Assert.Equal(expectedPath, fileRepository[0]);
    }

    [Fact]
    public void AddMod_ShouldOverwriteFilePath_WhenRepositoryAlreadyContainsFilePath()
    {
        // Arrange
        string folder = VirtualAssetRepository.AssetFolderNames[0];
        const string fileName = "test";

        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(folder)
            .AddFile(fileName);

        using TemporaryDirectory otherModDir = new();
        Mod otherMod = CreateMod(otherModDir);
        otherModDir.Root
            .AddFolder(folder)
            .AddFile(fileName, out string expectedPath);

        // Act
        _testTarget.AddMod(mod);
        _testTarget.AddMod(otherMod);

        // Assert
        List<string> fileRepository = GetFilePaths(_testTarget);
        Assert.Single(fileRepository);
        Assert.Equal(expectedPath, fileRepository.First());
    }

    [Fact]
    public void AddMod_ShouldSkipFilesOutsideAssetFolders()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFile("fileA.txt")
            .AddFolder("NonAsset")
            .AddFile("fileB.txt")
            .AddFolder("Nested")
            .AddFile("FileC.txt");

        // Act
        _testTarget.AddMod(mod);

        // Assert
        Assert.Empty(GetFilePaths(_testTarget));
    }

    [Fact]
    public void AddMod_ShouldThrowFileNotFoundException_WhenInvalidModsPath()
    {
        Mod modWithBadPath = new Mod("mod", Guid.NewGuid().ToString(), false);
        Assert.Throws<FileNotFoundException>(() => _testTarget.AddMod(modWithBadPath));
    }

    [Fact]
    public void CreateVirtualArchives_ShouldReturnSingleArchive_WhenRepositoryFitsInSingleArchive()
    {
        // Arrange
        _modDir.Root
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile("fileA.txt")
            .AddFolder("nested")
            .AddFile("fileB.txt");

        _testTarget.AddMod(CreateMod(_modDir));

        // Act
        VirtualArchive archive = _testTarget.CreateVirtualArchives().First();

        // Assert
        Assert.Equal(GetFilePaths(_testTarget), archive.EnumerateFilePaths().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldReturnArchiveWithSpecifiedSize()
    {
        // Arrange
        _modDir.Root
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile();

        _testTarget.AddMod(CreateMod(_modDir));
        _testTarget.ArchiveSizeInBytes = 512;

        // Act
        VirtualArchive archive = _testTarget.CreateVirtualArchives().First();

        // Assert
        Assert.Equal(_testTarget.ArchiveSizeInBytes, archive.MaxCapacityBytes);
    }

    [Fact]
    public void CreateVirtualArchives_ShouldNotReturnArchive_WhenRepositoryEmpty()
    {
        Assert.Empty(_testTarget.CreateVirtualArchives().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldReturnMultipleArchives_WhenRepositoryTooLargeForSingleArchive()
    {
        // Arrange
        _testTarget.ArchiveSizeInBytes = 256;
        _modDir.Root
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile(100)
            .AddFile(100)
            .AddFile(100);

        _testTarget.AddMod(CreateMod(_modDir));

        // Act
        VirtualArchive[] archives = _testTarget.CreateVirtualArchives().ToArray();

        // Assert
        Assert.Equal(2, archives.Length);
        Assert.Equal(2, archives[0].EnumerateFilePaths().ToArray().Length);
        Assert.Single(archives[1].EnumerateFilePaths().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldThrowFileNotFoundException_WhenRepositoryFileDeleted()
    {
        // Arrange
        _modDir.Root
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile("toDelete.txt", out string filePath);

        _testTarget.AddMod(CreateMod(_modDir));
        File.Delete(filePath);

        // Assert
        Assert.Throws<FileNotFoundException>(() => _testTarget.CreateVirtualArchives().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldThrowInvalidOperationException_WhenFileSizeExceedsArchiveSize()
    {
        // Arrange
        _testTarget.ArchiveSizeInBytes = 256;
        _modDir.Root
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile(512);

        _testTarget.AddMod(CreateMod(_modDir));

        // Assert
        Assert.Throws<InvalidOperationException>(() => _testTarget.CreateVirtualArchives().ToArray());
    }
}
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;

namespace MO2AutoPacker.Library.Tests;

// TODO: Refactor DRY.
public sealed class VirtualAssetRepositoryTests : IDisposable
{
    private readonly VirtualAssetRepository _testTarget = new();
    private readonly TemporaryDirectory _modDir = new();

    public void Dispose() => _modDir.Dispose();

    private static Mod CreateMod(TemporaryDirectory rootDir, bool isEnabled = false)
    {
        var modName = Guid.NewGuid().ToString();
        string modPath = Path.Combine(rootDir.Root.Directory.FullName, modName);

        DirectoryInfo modDir = new(modPath);
        return new Mod(modName, modDir, isEnabled);
    }

    [Fact]
    public void AddMod_ShouldAddFilePathsFromAssetFolders()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        List<string> filePaths = new();

        foreach (string folderName in VirtualAssetRepository.AssetFolderNames)
        {
            _modDir.Root
                .AddFolder(mod.Name)
                .AddFolder(folderName)
                .AddFile(out string filePath);

            filePaths.Add(filePath);
        }

        // Act
        _testTarget.AddMod(mod);

        // Assert
        // Ensure vfs repository contains the path of each asset file exactly once.
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        string[] assetPaths = assetFiles.Select(a => a.FullName).ToArray();
        Assert.Equal(filePaths.Count, assetFiles.Length);
        Assert.Empty(filePaths.Except(assetPaths));
    }

    [Fact]
    public void AddMod_ShouldAddFilePathsFromNestedAssetFolders()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFolder("nested")
            .AddFile("file.txt", out string expectedPath);

        // Act
        _testTarget.AddMod(mod);

        // Assert
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        Assert.Single(assetFiles);
        Assert.Equal(expectedPath, assetFiles[0].FullName);
    }

    [Fact]
    public void AddMod_ShouldOverwriteFilePath_WhenRepositoryAlreadyContainsFilePath()
    {
        // Arrange
        string folder = VirtualAssetRepository.AssetFolderNames[0];
        const string fileName = "test";

        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(folder)
            .AddFile(fileName);

        using TemporaryDirectory otherModDir = new();
        Mod otherMod = CreateMod(otherModDir);
        otherModDir.Root
            .AddFolder(otherMod.Name)
            .AddFolder(folder)
            .AddFile(fileName, out string expectedPath);

        // Act
        _testTarget.AddMod(mod);
        _testTarget.AddMod(otherMod);

        // Assert
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        Assert.Single(assetFiles);
        Assert.Equal(expectedPath, assetFiles.First().FullName);
    }

    [Fact]
    public void AddMod_ShouldSkipFilesOutsideAssetFolders()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFile("fileA.txt")
            .AddFolder("NonAsset")
            .AddFile("fileB.txt")
            .AddFolder("Nested")
            .AddFile("FileC.txt");

        // Act
        _testTarget.AddMod(mod);

        // Assert
        Assert.Empty(_testTarget.GetAssetFiles());
    }

    [Fact]
    public void AddMod_ShouldThrowDirectoryNotFoundException_WhenInvalidModDirectory()
    {
        // Don't create the mod folder in the temporary directory.
        Mod fakeMod = CreateMod(_modDir);
        Assert.Throws<DirectoryNotFoundException>(() => _testTarget.AddMod(fakeMod));
    }

    [Fact]
    public void CreateVirtualArchives_ShouldReturnSingleArchive_WhenRepositoryFitsInSingleArchive()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile("fileA.txt")
            .AddFolder("nested")
            .AddFile("fileB.txt");

        _testTarget.AddMod(mod);

        // Act
        VirtualArchive archive = _testTarget.CreateVirtualArchives().First();

        // Assert
        string[] assetPaths = _testTarget.GetAssetFiles().Select(a => a.FullName).ToArray();
        Assert.Equal(assetPaths, archive.EnumerateFilePaths().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldReturnArchiveWithSpecifiedSize()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile();

        _testTarget.AddMod(mod);
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
        Mod mod = CreateMod(_modDir);
        _testTarget.ArchiveSizeInBytes = 256;
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile(100)
            .AddFile(100)
            .AddFile(100);

        _testTarget.AddMod(mod);

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
        Mod mod = CreateMod(_modDir);
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile("toDelete.txt", out string filePath);

        _testTarget.AddMod(mod);
        File.Delete(filePath);

        // Assert
        Assert.Throws<FileNotFoundException>(() => _testTarget.CreateVirtualArchives().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldThrowInvalidOperationException_WhenFileSizeExceedsArchiveSize()
    {
        // Arrange
        Mod mod = CreateMod(_modDir);
        _testTarget.ArchiveSizeInBytes = 256;
        _modDir.Root
            .AddFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile(512);

        _testTarget.AddMod(mod);

        // Assert
        Assert.Throws<InvalidOperationException>(() => _testTarget.CreateVirtualArchives().ToArray());
    }
}
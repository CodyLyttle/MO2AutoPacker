using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;

namespace MO2AutoPacker.Library.Tests.Unit.Services;

// TODO: Refactor DRY.
public sealed class VirtualAssetRepositoryTests : IDisposable
{
    private readonly TemporaryDirectoryManager _tempDirManager;
    private readonly VirtualAssetRepository _testTarget;

    public VirtualAssetRepositoryTests()
    {
        _tempDirManager = new TemporaryDirectoryManager();
        _testTarget = new VirtualAssetRepository(_tempDirManager);
    }

    public void Dispose() => _tempDirManager.Dispose();

    private static Mod GetRandomMod() => new(Guid.NewGuid().ToString(), Random.Shared.Next(2) == 0);

    [Fact]
    public void AddMod_ShouldAddFilePathsFromAssetFolders()
    {
        // Arrange
        Mod mod = GetRandomMod();
        List<FileInfo> files = new();

        foreach (string assetFolderName in VirtualAssetRepository.AssetFolderNames)
        {
            _tempDirManager
                .AddModFolder(mod.Name)
                .AddFolder(assetFolderName)
                .AddFile(out FileInfo file);

            files.Add(file);
        }

        // Act
        _testTarget.AddMod(mod);

        // Assert
        // Ensure vfs repository contains every asset file.
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        string[] assetPaths = assetFiles.Select(a => a.FullName).ToArray();
        string[] filePaths = files.Select(f => f.FullName).ToArray();
        Assert.Equal(files.Count, assetFiles.Length);
        Assert.Empty(filePaths.Except(assetPaths));
    }

    [Fact]
    public void AddMod_ShouldAddFilePathsFromAssetFolders_WhenAssetFolderCaseMismatch()
    {
        // Arrange
        Mod mod = GetRandomMod();

        // Randomize lower/upper case of asset folder name. 
        char[] assetFolderChars = VirtualAssetRepository.AssetFolderNames[0].ToArray();
        for (var i = 0; i < assetFolderChars.Length; i++)
        {
            assetFolderChars[i] = i % 2 == 0
                    ? char.ToLower(assetFolderChars[i])
                    : char.ToUpper(assetFolderChars[i]);
        }
        var assetFolderName = new string(assetFolderChars);
        
        _tempDirManager
            .AddModFolder(mod.Name)
            .AddFolder(assetFolderName)
            .AddFile(out FileInfo expected);
        
        // Act
        _testTarget.AddMod(mod);

        // Assert
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        Assert.Single(assetFiles);
        Assert.Equal(expected.FullName, assetFiles[0].FullName);
    }

    [Fact]
    public void AddMod_ShouldAddFilePathsFromNestedAssetFolders()
    {
        // Arrange
        Mod mod = GetRandomMod();
        _tempDirManager
            .AddModFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFolder("nested")
            .AddFile("file.txt", out FileInfo file);

        // Act
        _testTarget.AddMod(mod);

        // Assert
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        Assert.Single(assetFiles);
        Assert.Equal(file.FullName, assetFiles[0].FullName);
    }

    [Fact]
    public void AddMod_ShouldOverwriteFilePath_WhenRepositoryAlreadyContainsFilePath()
    {
        // Arrange
        string assetFolderName = VirtualAssetRepository.AssetFolderNames[0];
        const string fileName = "test";

        Mod mod = GetRandomMod();
        _tempDirManager
            .AddModFolder(mod.Name)
            .AddFolder(assetFolderName)
            .AddFile(fileName);

        Mod otherMod = GetRandomMod();
        _tempDirManager
            .AddModFolder(otherMod.Name)
            .AddFolder(assetFolderName)
            .AddFile(fileName, out FileInfo file);

        // Act
        _testTarget.AddMod(mod);
        _testTarget.AddMod(otherMod);

        // Assert
        FileInfo[] assetFiles = _testTarget.GetAssetFiles();
        Assert.Single(assetFiles);
        Assert.Equal(file.FullName, assetFiles.First().FullName);
    }

    [Fact]
    public void AddMod_ShouldSkipFilesOutsideAssetFolders()
    {
        // Arrange
        Mod mod = GetRandomMod();
        _tempDirManager
            .AddModFolder(mod.Name)
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
        Mod fakeMod = GetRandomMod();
        Assert.Throws<DirectoryNotFoundException>(() => _testTarget.AddMod(fakeMod));
    }

    [Fact]
    public void CreateVirtualArchives_ShouldReturnSingleArchive_WhenRepositoryFitsInSingleArchive()
    {
        // Arrange
        Mod mod = GetRandomMod();
        _tempDirManager
            .AddModFolder(mod.Name)
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
        Mod mod = GetRandomMod();
        _tempDirManager
            .AddModFolder(mod.Name)
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
    public void CreateVirtualArchives_ShouldNotReturnArchive_WhenRepositoryEmpty() =>
        Assert.Empty(_testTarget.CreateVirtualArchives().ToArray());

    [Fact]
    public void CreateVirtualArchives_ShouldReturnMultipleArchives_WhenRepositoryTooLargeForSingleArchive()
    {
        // Arrange
        Mod mod = GetRandomMod();
        _testTarget.ArchiveSizeInBytes = 256;
        _tempDirManager
            .AddModFolder(mod.Name)
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
        Mod mod = GetRandomMod();
        _tempDirManager
            .AddModFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile("toDelete.txt", out FileInfo file);

        _testTarget.AddMod(mod);
        file.Delete();

        // Assert
        Assert.Throws<FileNotFoundException>(() => _testTarget.CreateVirtualArchives().ToArray());
    }

    [Fact]
    public void CreateVirtualArchives_ShouldThrowInvalidOperationException_WhenFileSizeExceedsArchiveSize()
    {
        // Arrange
        Mod mod = GetRandomMod();
        _testTarget.ArchiveSizeInBytes = 256;
        _tempDirManager
            .AddModFolder(mod.Name)
            .AddFolder(VirtualAssetRepository.AssetFolderNames[0])
            .AddFile(512);

        _testTarget.AddMod(mod);

        // Assert
        Assert.Throws<InvalidOperationException>(() => _testTarget.CreateVirtualArchives().ToArray());
    }
}
using FluentAssertions;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;

namespace MO2AutoPacker.Library.Tests.Unit;

public class DirectoryManagerTests
{
    private readonly DirectoryManager _testTarget;
    private readonly TemporaryDirectory _tempDir;
    private readonly TemporaryFolder _modsFolder;
    private readonly TemporaryFolder _profilesFolder;

    public DirectoryManagerTests()
    {
        _tempDir = new TemporaryDirectory();
        _modsFolder = _tempDir.Root.AddFolder("mods");
        _profilesFolder = _tempDir.Root.AddFolder("profiles");
        _testTarget = new DirectoryManager();
        _testTarget.SetModOrganizer(_tempDir.Root.Directory.FullName);
    }

    private static void AssertEqualPath(TemporaryFolder expected, DirectoryInfo actual) =>
        Assert.Equal(expected.Directory.FullName, actual.FullName);

    [Fact]
    public void GetModOrganizer_ShouldReturnRootFolder() =>
        AssertEqualPath(_tempDir.Root, _testTarget.GetModOrganizer());

    [Fact]
    public void GetMods_ShouldReturnModsFolder() => AssertEqualPath(_modsFolder, _testTarget.GetModsFolder());

    [Fact]
    public void GetProfiles_ShouldReturnProfilesFolder() =>
        AssertEqualPath(_profilesFolder, _testTarget.GetProfilesFolder());

    [Fact]
    public void GetModFolders_ShouldEnumerateAllMods()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
            _modsFolder.AddFolder("Test Mod " + i);

        // Act
        DirectoryInfo[] expectedMods = _modsFolder.Directory.GetDirectories();
        DirectoryInfo[] actualMods = _testTarget.GetModFolders().ToArray();

        // Assert
        Assert.Equal(expectedMods.Length, actualMods.Length);
        for (var i = 0; i < expectedMods.Length; i++)
            Assert.Equal(expectedMods[i].FullName, actualMods[i].FullName);
    }

    [Fact]
    public void GetProfileFolders_ShouldEnumerateAllProfiles()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
            _profilesFolder.AddFolder("Test Profile " + i);

        // Act
        DirectoryInfo[] expectedProfiles = _profilesFolder.Directory.GetDirectories();
        DirectoryInfo[] actualProfiles = _testTarget.GetProfileFolders().ToArray();

        // Assert
        Assert.Equal(expectedProfiles.Length, actualProfiles.Length);
        for (var i = 0; i < expectedProfiles.Length; i++)
            Assert.Equal(expectedProfiles[i].FullName, actualProfiles[i].FullName);
    }

    [Fact]
    public void GetModFolder_ShouldReturnModFolder_WhenModFolderExists()
    {
        // Arrange
        const string modName = "Real mod";
        _modsFolder.AddFolder(modName);
        
        // Act
        _testTarget.GetModFolder(modName);
    }

    [Fact]
    public void GetModFolder_ShouldThrowDirectoryNotFoundException_WhenModFolderMissing()
    {
        // Arrange
        const string modName = "Fake mod";
        Action act = () => _testTarget.GetModFolder(modName);

        // Assert
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage($"*{modName}*");
    }

    [Fact]
    public void SetModOrganizer_ShouldThrowDirectoryNotFoundException_WhenPathDoesNotExist()
    {
        // Arrange
        Action act = () => _testTarget.SetModOrganizer(@"I:\My\Fake\Path");

        // Act
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage("*root*");
    }

    [Fact]
    public void SetModOrganizer_ShouldThrowDirectoryNotFoundException_WhenMissingModsFolder()
    {
        // Arrange
        _modsFolder.Directory.Delete();
        Action act = () => _testTarget.SetModOrganizer(_tempDir.Root.Directory.FullName);

        // Assert
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage("*'mods'*");
    }

    [Fact]
    public void SetModOrganizer_ShouldThrowDirectoryNotFoundException_WhenMissingProfilesFolder()
    {
        // Arrange
        _profilesFolder.Directory.Delete();
        Action act = () => _testTarget.SetModOrganizer(_tempDir.Root.Directory.FullName);

        // Assert
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage("*'profiles'*");
    }

    [Fact]
    public void VariousGetters_ShouldThrowInvalidOperationException_BeforeCallingRespectiveInitializer()
    {
        // Arrange
        DirectoryManager uninitialized = new();
        Action[] actions =
        {
            () => _ = uninitialized.GetModOrganizer(),
            () => _ = uninitialized.GetModsFolder(),
            () => _ = uninitialized.GetModFolders(),
            () => _ = uninitialized.GetModFolder(""),
            () => _ = uninitialized.GetProfilesFolder(),
            () => _ = uninitialized.GetProfilesFolder()
        };

        // Assert
        foreach (Action act in actions)
        {
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*uninitialized*");
        }
    }

    [Fact]
    public void VariousGetters_ShouldThrowDirectoryNotFoundException_WhenDirectoryRemovedAfterInitialized()
    {
        // Arrange
        _tempDir.Root.Directory.Delete(true);
        (DirectoryInfo, Action)[] actions =
        {
            (_tempDir.Root.Directory, () => _ = _testTarget.GetModOrganizer()),
            (_modsFolder.Directory, () => _ = _testTarget.GetModsFolder()),
            (_modsFolder.Directory, () => _ = _testTarget.GetModFolders()),
            (_modsFolder.Directory, () => _ = _testTarget.GetModFolder("")),
            (_profilesFolder.Directory, () => _ = _testTarget.GetProfilesFolder()),
            (_profilesFolder.Directory, () => _ = _testTarget.GetProfilesFolder())
        };

        // Assert
        foreach ((DirectoryInfo dir, Action act) in actions)
        {
            act.Should().Throw<DirectoryNotFoundException>()
                .WithMessage(dir.FullName);
        }
    }
}
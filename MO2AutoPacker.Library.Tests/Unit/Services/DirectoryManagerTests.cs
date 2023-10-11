using FluentAssertions;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;
using MO2AutoPacker.Library.Tests.Stubs;

namespace MO2AutoPacker.Library.Tests.Unit.Services;

public class DirectoryManagerTests
{
    private const string ArchiverFolderName = "Archiver";
    private const string ModOrganizerFolderName = "ModOrganizer";
    private readonly FileInfo _archiverExecutable;

    private readonly TemporaryFolder _archiverFolder;
    private readonly TemporaryFolder _modOrganizerFolder;
    private readonly TemporaryFolder _modsFolder;
    private readonly PathReaderStub _pathReader;
    private readonly TemporaryFolder _profilesFolder;
    private readonly TemporaryDirectory _tempDir;
    private readonly DirectoryManager _testTarget;

    public DirectoryManagerTests()
    {
        _tempDir = new TemporaryDirectory();

        _archiverFolder = _tempDir.Root.AddFolder(ArchiverFolderName);
        _archiverFolder.AddFile(DirectoryManager.ArchiverExecutableName);
        _archiverExecutable = _archiverFolder.Directory.GetFiles()[0];

        _modOrganizerFolder = _tempDir.Root.AddFolder(ModOrganizerFolderName);
        _modsFolder = _modOrganizerFolder.AddFolder(DirectoryManager.ModsFolderName);
        _profilesFolder = _modOrganizerFolder.AddFolder(DirectoryManager.ProfilesFolderName);

        _pathReader = new PathReaderStub();
        _testTarget = new DirectoryManager(_pathReader);
        _testTarget.SetArchiverFolder(_archiverFolder.Directory.FullName);
        _testTarget.SetModOrganizerFolder(_modOrganizerFolder.Directory.FullName);
    }

    private static void AssertEqualPath(TemporaryFolder expected, DirectoryInfo actual) =>
        Assert.Equal(expected.Directory.FullName, actual.FullName);

    [Fact]
    public void GetArchiverFolder_ShouldReturnArchiverFolder() =>
        AssertEqualPath(_archiverFolder, _testTarget.GetArchiverFolder());

    [Fact]
    public void GetArchiverExecutable_ShouldReturnArchiverExecutableFile() =>
        Assert.Equal(_archiverFolder.Directory.GetFiles()[0].FullName, _testTarget.GetArchiverExecutable().FullName);

    [Fact]
    public void GetModOrganizer_ShouldReturnRootFolder() =>
        AssertEqualPath(_modOrganizerFolder, _testTarget.GetModOrganizerFolder());

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
        Action act = () => _testTarget.SetModOrganizerFolder(@"I:\My\Fake\Path");

        // Act
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage("*root*");
    }

    [Fact]
    public void SetModOrganizer_ShouldThrowDirectoryNotFoundException_WhenMissingModsFolder()
    {
        // Arrange
        _modsFolder.Directory.Delete();
        Action act = () => _testTarget.SetModOrganizerFolder(_tempDir.Root.Directory.FullName);

        // Assert
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage("*'mods'*");
    }

    [Fact]
    public void SetModOrganizer_ShouldThrowDirectoryNotFoundException_WhenMissingProfilesFolder()
    {
        // Arrange
        _profilesFolder.Directory.Delete();
        Action act = () => _testTarget.SetModOrganizerFolder(_modOrganizerFolder.Directory.FullName);

        // Assert
        act.Should().Throw<DirectoryNotFoundException>()
            .WithMessage("*'profiles'*");
    }

    [Fact]
    public void VariousGetters_ShouldThrowInvalidOperationException_BeforeCallingRespectiveInitializer()
    {
        // Arrange
        DirectoryManager uninitialized = new(_pathReader);
        Action[] actions =
        {
            () => _ = uninitialized.GetArchiverFolder(),
            () => _ = uninitialized.GetModOrganizerFolder(),
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
    public void GetArchiverExecutable_ShouldThrowFileNotFoundException_WhenFileRemovedAfterInitialized()
    {
        // Arrange
        _archiverExecutable.Delete();

        // Act
        Action act = () => _testTarget.GetArchiverExecutable();

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage(_archiverExecutable.FullName);
    }

    [Fact]
    public void VariousGetters_ShouldThrowDirectoryNotFoundException_WhenDirectoryRemovedAfterInitialized()
    {
        // Arrange
        _tempDir.Root.Directory.Delete(true);
        (DirectoryInfo, Action)[] actions =
        {
            (_archiverFolder.Directory, () => _ = _testTarget.GetArchiverFolder()),
            (_modOrganizerFolder.Directory, () => _ = _testTarget.GetModOrganizerFolder()),
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
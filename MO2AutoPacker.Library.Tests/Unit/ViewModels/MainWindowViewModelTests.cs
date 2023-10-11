using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;
using MO2AutoPacker.Library.Tests.Stubs;
using MO2AutoPacker.Library.ViewModels;
using Moq;

namespace MO2AutoPacker.Library.Tests.Unit.ViewModels;

public class MainWindowViewModelTests
{
    private const string InitialPath = @"C:\InitialPath";
    private const string UpdatedPath = @"C:\UpdatedPath";

    private readonly IMessenger _messenger;
    private readonly Mock<IDirectoryManager> _mockDirectoryManager;
    private readonly ModListReader _modListReader;
    private readonly PathPickerStub _pathPicker;
    private MainWindowViewModel _testTarget;

    public MainWindowViewModelTests()
    {
        _messenger = new WeakReferenceMessenger();
        _pathPicker = new PathPickerStub();
        _mockDirectoryManager = new Mock<IDirectoryManager>();
        _modListReader = new ModListReader(_mockDirectoryManager.Object);
        _testTarget = new MainWindowViewModel(_messenger, _pathPicker, _mockDirectoryManager.Object,
            _modListReader);
    }

    [Fact]
    public void Constructor_ShouldSetEmptyPaths_WhenPathFetcherDoesNotHavePaths()
    {
        Assert.Equal(string.Empty, _testTarget.ArchiverPath);
        Assert.Equal(string.Empty, _testTarget.ModOrganizerPath);
    }

    [Fact]
    public void Constructor_ShouldSetPathsUsingPathFetcher_WhenPathFetcherHasPaths()
    {
        // Arrange
        TemporaryDirectoryManager tempDir = new();
        _mockDirectoryManager.Setup(x => x.IsArchiverDirectoryInitialized).Returns(true);
        _mockDirectoryManager.Setup(x => x.IsModOrganizerDirectoryInitialized).Returns(true);
        _mockDirectoryManager.Setup(x => x.GetArchiverFolder()).Returns(tempDir.GetArchiverFolder);
        _mockDirectoryManager.Setup(x => x.GetModOrganizerFolder()).Returns(tempDir.GetModOrganizerFolder);
        _testTarget = new MainWindowViewModel(_messenger, _pathPicker, _mockDirectoryManager.Object,
            _modListReader);

        // Assert
        Assert.Equal(tempDir.GetArchiverFolder().FullName, _testTarget.ArchiverPath);
        Assert.Equal(tempDir.GetModOrganizerFolder().FullName, _testTarget.ModOrganizerPath);
    }

    [Fact]
    public void PickArchiverPathCommand_CommonTests()
    {
        CommonPathPickerTests tester = new(_pathPicker,
            () => _testTarget.ArchiverPath,
            path => _testTarget.ArchiverPath = path,
            _testTarget.PickArchiverPathCommand);

        tester.AssertAll();
    }

    [Fact]
    public void PickArchiverPathCommand_ShouldUpdateDirectoryManager_WhenPathChanged()
    {
        // Arrange
        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);

        // Act
        _testTarget.PickArchiverPathCommand.Execute(null);

        // Assert
        _mockDirectoryManager.Verify(x => x.SetArchiverFolder(UpdatedPath), Times.Once);
    }

    [Fact]
    public void PickArchiverPathCommand_ShouldSendBannerMessage_WhenInvalidArchiverPath()
    {
        // Arrange
        MessageCollector messageCollector = new MessageCollector(_messenger)
            .AddWhitelist<BannerMessage>();

        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);
        _mockDirectoryManager.Setup(x => x.SetArchiverFolder(UpdatedPath))
            .Throws(new DirectoryNotFoundException(UpdatedPath));

        // Act
        _testTarget.PickArchiverPathCommand.Execute(null);

        // Assert
        var bannerMsg = messageCollector.DequeueMessage<BannerMessage>();
        Assert.Equal(BannerMessage.Type.Error, bannerMsg.MessageType);
        Assert.Equal(UpdatedPath, bannerMsg.Message);
    }

    [Fact]
    public void PickModOrganizerPathCommand_CommonTests()
    {
        CommonPathPickerTests tester = new(_pathPicker,
            () => _testTarget.ModOrganizerPath,
            path => _testTarget.ModOrganizerPath = path,
            _testTarget.PickModOrganizerPathCommand);

        tester.AssertAll();
    }

    [Fact]
    public void PickModOrganizerPathCommand_ShouldUpdateDirectoryManager_WhenPathChanged()
    {
        // Arrange
        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);

        // Act
        _testTarget.PickModOrganizerPathCommand.Execute(null);

        // Assert
        _mockDirectoryManager.Verify(x => x.SetModOrganizerFolder(UpdatedPath), Times.Once);
    }

    [Fact]
    public void PickModOrganizerPathCommand_ShouldSendModOrganizerPathChangedMessage_WhenPathChanged()
    {
        // Arrange
        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);
        MessageCollector messageCollector = new MessageCollector(_messenger)
            .AddWhitelist<ModOrganizerPathChangedMessage>();

        // Act
        _testTarget.PickModOrganizerPathCommand.Execute(null);

        // Assert
        messageCollector.AssertCount<ModOrganizerPathChangedMessage>(1);
    }

    [Fact]
    public void PickModOrganizerPathCommand_ShouldSendBannerMessage_WhenInvalidModOrganizerPath()
    {
        // Arrange
        MessageCollector messageCollector = new MessageCollector(_messenger)
            .AddWhitelist<BannerMessage>();

        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);
        _mockDirectoryManager.Setup(x => x.SetModOrganizerFolder(UpdatedPath))
            .Throws(new DirectoryNotFoundException(UpdatedPath));

        // Act
        _testTarget.PickModOrganizerPathCommand.Execute(null);

        // Assert
        var bannerMsg = messageCollector.DequeueMessage<BannerMessage>();
        messageCollector.AssertEmpty();
        Assert.Equal(BannerMessage.Type.Error, bannerMsg.MessageType);
        Assert.Equal(UpdatedPath, bannerMsg.Message);
    }

    public class CommonPathPickerTests
    {
        private readonly ICommand _executor;
        private readonly Func<string> _pathGetter;
        private readonly PathPickerStub _pathPicker;
        private readonly Action<string> _pathSetter;

        public CommonPathPickerTests(PathPickerStub pathPicker, Func<string> pathGetter,
            Action<string> pathSetter, ICommand executor)
        {
            _pathPicker = pathPicker;
            _pathGetter = pathGetter;
            _pathSetter = pathSetter;
            _executor = executor;
        }

        public void AssertAll()
        {
            Action[] tests =
            {
                ShouldUpdatePath_WhenPathChanged,
                ShouldDoNothing_WhenPathIsNull,
                ShouldDoNothing_WhenPathIsIdentical
            };

            foreach (Action test in tests)
            {
                _pathSetter(InitialPath);
                test.Invoke();
            }
        }

        private void ShouldUpdatePath_WhenPathChanged()
        {
            // Arrange 
            _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);

            // Act
            _executor.Execute(null);

            // Assert
            Assert.Equal(UpdatedPath, _pathGetter());
        }

        private void ShouldDoNothing_WhenPathIsNull()
        {
            // Arrange
            _pathPicker.DirectoryToReturn = null;

            // Act
            _executor.Execute(null);

            // Assert
            Assert.Equal(InitialPath, _pathGetter());
        }

        private void ShouldDoNothing_WhenPathIsIdentical()
        {
            // Arrange
            _pathPicker.DirectoryToReturn = new DirectoryInfo(InitialPath);

            // Act
            _executor.Execute(null);

            // Assert
            Assert.Equal(InitialPath, _pathGetter());
        }
    }
}
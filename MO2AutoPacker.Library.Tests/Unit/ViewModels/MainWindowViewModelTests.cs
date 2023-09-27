using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Tests.Helpers;
using MO2AutoPacker.Library.Tests.Stubs;
using MO2AutoPacker.Library.ViewModels;
using Moq;

namespace MO2AutoPacker.Library.Tests.Unit.ViewModels;

public class MainWindowViewModelTests
{
    private const string InitialPath = @"C:\Existing\Path";
    private const string UpdatedPath = @"D:\New\Path";

    private readonly IMessenger _messenger;
    private readonly Mock<IDirectoryManager> _mockDirectoryManager;
    private readonly PathPickerStub _pathPicker;
    private readonly MainWindowViewModel _testTarget;

    public MainWindowViewModelTests()
    {
        _messenger = new WeakReferenceMessenger();
        _pathPicker = new PathPickerStub();
        _mockDirectoryManager = new Mock<IDirectoryManager>();
        _testTarget = new MainWindowViewModel(_messenger, _pathPicker, _mockDirectoryManager.Object)
        {
            ModOrganizerPath = InitialPath
        };
    }

    [Fact]
    public void PickModOrganizerPathCommand_ShouldUpdatePath_WhenPathChanged()
    {
        // Arrange 
        MessageCollector messageCollector = new MessageCollector(_messenger)
            .AddWhitelist<ModOrganizerPathChangedMessage>();

        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);

        // Act
        _testTarget.PickModOrganizerPathCommand.Execute(null);

        // Assert
        Assert.Equal(UpdatedPath, _testTarget.ModOrganizerPath);
        messageCollector.AssertCount<ModOrganizerPathChangedMessage>(1);
        _mockDirectoryManager.Verify(x => x.SetModOrganizerFolder(UpdatedPath), Times.Once);
    }

    [Fact]
    public void PickModOrganizerPathCommand_ShouldDoNothing_WhenPathIsNullOrIdentical()
    {
        // Arrange
        MessageCollector messageCollector = new MessageCollector(_messenger)
            .AddWhitelist<BannerMessage>()
            .AddWhitelist<ModOrganizerPathChangedMessage>();

        _pathPicker.DirectoryToReturn = null;

        // Act
        _testTarget.PickModOrganizerPathCommand.Execute(null);

        // Assert
        messageCollector.AssertEmpty();
        Assert.Equal(InitialPath, _testTarget.ModOrganizerPath);
    }

    [Fact]
    public void PickModOrganizerPathCommand_ShouldSendBannerError_WhenInvalidPath()
    {
        // Arrange
        MessageCollector messageCollector = new MessageCollector(_messenger)
            .AddWhitelist<BannerMessage>()
            .AddBlacklist<ModOrganizerPathChangedMessage>();

        _mockDirectoryManager.Setup(x => x.SetModOrganizerFolder(UpdatedPath))
            .Throws(new DirectoryNotFoundException(UpdatedPath));

        _pathPicker.DirectoryToReturn = new DirectoryInfo(UpdatedPath);

        // Act
        _testTarget.PickModOrganizerPathCommand.Execute(null);

        // Assert
        var bannerMsg = messageCollector.DequeueMessage<BannerMessage>();
        Assert.Equal(BannerMessage.Type.Error, bannerMsg.MessageType);
        Assert.Equal(UpdatedPath, bannerMsg.Message);
        messageCollector.AssertEmpty();
    }
}
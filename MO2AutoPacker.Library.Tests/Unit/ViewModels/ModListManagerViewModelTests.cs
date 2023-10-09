using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Tests.Helpers;
using MO2AutoPacker.Library.ViewModels;
using Moq;

namespace MO2AutoPacker.Library.Tests.Unit.ViewModels;

public sealed class ModListManagerViewModelTests : IDisposable
{
    private readonly MessageCollector _messageCollector;
    private readonly IMessenger _messenger;
    private readonly Mock<IModListReader> _mockReader;
    private readonly TemporaryDirectoryManager _tempDirManager;
    private readonly ModListManagerViewModel _testTarget;

    public ModListManagerViewModelTests()
    {
        _messenger = new WeakReferenceMessenger();
        _messageCollector = new MessageCollector(_messenger);
        _mockReader = new Mock<IModListReader>();
        _tempDirManager = new TemporaryDirectoryManager();
        _testTarget = new ModListManagerViewModel(_messenger, _mockReader.Object);
    }

    public void Dispose() => _tempDirManager.Dispose();

    private Profile CreateProfile() => new(_tempDirManager.AddProfileFolder().Directory);

    private void SendProfileChangedMessage(Profile profile) => _messenger.Send(new ProfileChangedMessage(profile));

    [Fact]
    public void Receive_ShouldUpdateModListUsingModListReader()
    {
        // Arrange
        Profile profile = CreateProfile();
        ModList expected = new(profile.Name, new ModSeparator("A"), new Mod("B", false), new Mod("C", true));
        _mockReader.Setup(x => x.Read(profile)).Returns(expected);

        // Act
        SendProfileChangedMessage(profile);

        // Assert
        Assert.NotNull(_testTarget.ModList);
        Assert.Equal(expected, _testTarget.ModList);
    }

    [Fact]
    public void Receive_ShouldSendBannerError_WhenModListReaderThrowsDirectoryNotFoundException()
    {
        // Arrange
        _messageCollector.AddWhitelist<BannerMessage>();
        DirectoryNotFoundException ex = new("Missing directory");
        Profile profile = CreateProfile();
        _mockReader.Setup(x => x.Read(profile)).Throws(ex);

        // Act
        SendProfileChangedMessage(profile);

        // Assert
        var message = _messageCollector.DequeueMessage<BannerMessage>();
        Assert.Equal(ex.Message, message.Message);
        _messageCollector.AssertEmpty();
    }

    [Fact]
    public void Receive_ShouldSendBannerError_WhenModListReaderThrowsFileNotFoundException()
    {
        // Arrange
        _messageCollector.AddWhitelist<BannerMessage>();
        FileNotFoundException ex = new("Missing file");
        Profile profile = CreateProfile();
        _mockReader.Setup(x => x.Read(profile)).Throws(ex);

        // Act
        SendProfileChangedMessage(profile);

        // Assert
        var message = _messageCollector.DequeueMessage<BannerMessage>();
        Assert.Equal(ex.Message, message.Message);
        _messageCollector.AssertEmpty();
    }
}
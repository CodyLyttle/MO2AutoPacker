using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Tests.Helpers;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.Library.Tests;

public sealed class ModListManagerViewModelTests : IDisposable
{
    private const string ModListFileName = "modlist.txt";

    private readonly IMessenger _messenger;
    private readonly TemporaryFolder _modsFolder;
    private readonly TemporaryFolder _profilesFolder;
    private readonly TemporaryDirectory _tempDir;
    private readonly ModListManagerViewModel _testTarget;

    public ModListManagerViewModelTests()
    {
        _messenger = new WeakReferenceMessenger();
        _testTarget = new ModListManagerViewModel(_messenger);
        _tempDir = new TemporaryDirectory();
        _modsFolder = _tempDir.Root.AddFolder("mods");
        _profilesFolder = _tempDir.Root.AddFolder("profiles");
    }

    public void Dispose() => _tempDir.Dispose();

    private Profile CreateProfile()
    {
        var profileName = Guid.NewGuid().ToString();
        TemporaryFolder profileFolder = _profilesFolder.AddFolder(profileName);
        return new Profile(profileFolder.Directory);
    }

    private Profile AddModListFile(Profile profile, int modCount)
    {
        string filePath = Path.Combine(profile.Directory.FullName, ModListFileName);
        using TextWriter writer = File.CreateText(filePath);
        for (var i = 0; i < modCount; i++)
        {
            // 1/3 deactivated, 2/3 activated.
            char prefix = Random.Shared.Next(0, 3) == 0
                ? '-'
                : '+';

            writer.Write(prefix);
            writer.WriteLine(Guid.NewGuid().ToString());
        }

        return profile;
    }

    [Fact]
    public void Receive_ShouldReceiveProfileChangedMessages()
    {
        // Arrange
        Profile profile = AddModListFile(CreateProfile(), 10);
        ProfileChangedMessage outgoingMsg = new(profile);

        // Act
        _messenger.Send(outgoingMsg);

        // Assert
        Assert.Equal(profile.Name, _testTarget.ModList!.Name);
    }

    [Fact]
    public void Receive_ShouldSendBannerError_WhenMissingModListFile()
    {
        // Arrange
        BannerMessageReceiver errorReceiver = new(_messenger);
        ProfileChangedMessage outgoingMsg = new(CreateProfile());

        // Act
        _testTarget.Receive(outgoingMsg);
        BannerMessage incomingMsg = errorReceiver.Messages.Dequeue();

        // Assert
        Assert.Equal(BannerMessage.Type.Error, incomingMsg.MessageType);
        Assert.Contains(ModListFileName, incomingMsg.Message);
    }
}
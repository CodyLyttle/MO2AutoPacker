using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Tests.Helpers;
using MO2AutoPacker.Library.Tests.Stubs;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.Library.Tests.Unit.ViewModels;

public sealed class ModListManagerViewModelTests : IDisposable
{
    private const string ModListFileName = "modlist.txt";

    private readonly IMessenger _messenger;
    private readonly TemporaryDirectoryManager _tempDirManager;
    private readonly ModListManagerViewModel _testTarget;

    public ModListManagerViewModelTests()
    {
        _messenger = new WeakReferenceMessenger();
        _tempDirManager = new TemporaryDirectoryManager();
        _testTarget = new ModListManagerViewModel(_messenger, _tempDirManager);
    }

    public void Dispose() => _tempDirManager.Dispose();

    private Profile CreateProfile() => new(_tempDirManager.AddProfileFolder().Directory);

    [Fact]
    public void Receive_ShouldReceiveProfileChangedMessages()
    {
        // Arrange
        Profile profile = CreateProfile();
        ModListBuilder.BuildRandom().WriteFile(profile.Directory);
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
        RecipientStub<BannerMessage> errorReceiver = new(_messenger);
        ProfileChangedMessage outgoingMsg = new(CreateProfile());

        // Act
        _testTarget.Receive(outgoingMsg);
        BannerMessage incomingMsg = errorReceiver.Messages.Dequeue();

        // Assert
        Assert.Equal(BannerMessage.Type.Error, incomingMsg.MessageType);
        Assert.Contains($"missing file '{ModListFileName}'", incomingMsg.Message);
    }

    [Fact]
    public void Receive_ShouldReadModsFromModList_WhenModListContains()
    {
        // Arrange
        Profile profile = CreateProfile();
        var builder = ModListBuilder.BuildRandom();
        builder.WriteFile(profile.Directory);
        ProfileChangedMessage outgoingMsg = new(profile);

        // Act
        _testTarget.Receive(outgoingMsg);

        // Assert
        Assert.NotNull(_testTarget.ModList);
        List<IModListItem> sourceItems = builder.ModListItems.ToList();
        foreach (IModListItem resultItem in _testTarget.ModList.Items)
        {
            for (var i = 0; i < sourceItems.Count; i++)
            {
                IModListItem sourceItem = sourceItems[i];
                if (resultItem.Name != sourceItem.Name)
                    continue;

                Debug.WriteLine("Matching name");
                if ((sourceItem is ModSeparator && resultItem is ModSeparator) ||
                    (sourceItem is Mod sourceMod && resultItem is Mod resultMod &&
                     sourceMod.IsEnabled == resultMod.IsEnabled))
                {
                    sourceItems.RemoveAt(i);
                    break;
                }
            }
        }

        // A perfectly parsed mod list results in every source item being matched and removed.
        Assert.Empty(sourceItems);
    }
}
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.Tests.Helpers;
using Moq;

namespace MO2AutoPacker.Library.Tests.Unit.Services;

public class ModListReaderTests
{
    [Fact]
    public void Read_ShouldReadModsFromModList()
    {
        // Arrange
        // Create temporary profile folder.
        TemporaryDirectory tempDir = new();
        TemporaryFolder profileFolder = tempDir.Root.AddFolder("myProfile");
        Profile profile = new(profileFolder.Directory);

        // Create temporary mod list file.
        var builder = ModListBuilder.BuildRandom();
        builder.WriteFile(profile.Directory, out FileInfo modListFile);

        // Setup mock reader to return temp mod list file.
        Mock<IDirectoryReader> mockDirectoryReader = new();
        mockDirectoryReader.Setup(x => x.GetModList(profile.Name))
            .Returns(modListFile);

        ModListReader testTarget = new(mockDirectoryReader.Object);
        List<IModListItem> expectedMods = builder.ModListItems.ToList();

        // Act
        ModList actual = testTarget.Read(profile);

        // Assert
        foreach (IModListItem actualItem in actual.Items)
        {
            IModListItem? match = expectedMods.FirstOrDefault(x => x.Equals(actualItem));
            if (match is null)
                throw new InvalidOperationException("Mod list item mismatch");

            expectedMods.Remove(match);
        }

        // A perfectly parsed mod list results in every source item being matched and removed.
        Assert.Empty(expectedMods);
    }
}
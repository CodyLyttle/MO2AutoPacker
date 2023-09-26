using System.Globalization;
using FluentAssertions;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Tests.Unit;

public class PackedMetaDataTests
{
    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        PackedMetaData testTarget = new("MyArchive", 1024, DateTime.Now, new[] {"modA", "modB", "modC"});

        string expected = $"archive={testTarget.ArchiveName}\r\n" +
                          $"size={testTarget.Size}\r\n" +
                          $"time={testTarget.PackedAt.ToString(DateTimeFormatInfo.InvariantInfo)}\r\n" +
                          $"{testTarget.Mods[0]}\r\n" +
                          $"{testTarget.Mods[1]}\r\n" +
                          $"{testTarget.Mods[2]}\r\n";

        // Act
        var actual = testTarget.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadFromString_ShouldReturnEquivalentObject_WhenValidString()
    {
        // Arrange
        PackedMetaData expected = new("MyArchive", 1024, DateTime.Now, new[] {"modA", "modB", "modC"});
        string input = $"archive={expected.ArchiveName}\r\n" +
                       $"size={expected.Size}\r\n" +
                       $"time={expected.PackedAt.ToString(DateTimeFormatInfo.InvariantInfo)}\r\n" +
                       $"{expected.Mods[0]}\r\n" +
                       $"{expected.Mods[1]}\r\n" +
                       $"{expected.Mods[2]}\r\n";

        // Act
        PackedMetaData actual = PackedMetaData.ReadFromString(input);

        // Assert
        Assert.Equal(expected.ArchiveName, actual.ArchiveName);
        Assert.Equal(expected.Size, actual.Size);
        Assert.Equal(expected.PackedAt.ToLongTimeString(), actual.PackedAt.ToLongTimeString());
        Assert.Equivalent(expected.Mods, actual.Mods);
    }

    [Fact]
    public void ReadFromString_ShouldThrowFormatException_WhenMissingHeaders()
    {
        const int headerCount = 3;

        (string Key, string Value)[] headersSource =
        {
            new("archive=", "MyArchive"),
            new("size=", "1024"),
            new("time=", "09/22/2023 02:04:31")
        };

        var headers = new (string Key, string Value)[headerCount];
        headersSource.CopyTo(headers, 0);

        for (var i = 0; i < headerCount; i++)
        {
            // Read with temporarily removed header key.
            headers[i].Key = string.Empty;
            Action act = () => PackedMetaData.ReadFromString(BuildInput(headers));
            act.Should().Throw<FormatException>().WithMessage($"Missing header '{headersSource[i].Key}'");
            headers[i].Key = headersSource[i].Key;

            // Read with temporarily removed header value.
            headers[i].Value = string.Empty;
            act = () => PackedMetaData.ReadFromString(BuildInput(headers));
            act.Should().Throw<FormatException>().WithMessage($"Missing value for header '{headersSource[i].Key}'");
            headers[i].Value = headersSource[i].Value;
        }

        string BuildInput(IReadOnlyList<(string Key, string Value)> headerPairs)
        {
            return $"{headerPairs[0].Key}{headerPairs[0].Value}\r\n" +
                   $"{headerPairs[1].Key}{headerPairs[1].Value}\r\n" +
                   $"{headerPairs[2].Key}{headerPairs[2].Value}\r\n" +
                   $"modA\r\n" +
                   $"modB\r\n" +
                   $"modC\r\n";
        }
    }

    [Fact]
    public void ReadFromString_ShouldThrowFormatException_WhenBadSizeFormat()
    {
        // Arrange
        const string input = "archive=MyArchive\r\n" +
                             "size=NotAnInt32\r\n" +
                             "time=09/22/2023 02:04:31\r\n";

        // Act/Assert
        Action act = () => PackedMetaData.ReadFromString(input);
        act.Should().Throw<FormatException>()
            .WithMessage("Expected Int32 value for header 'size='");
    }

    [Fact]
    public void ReadFromString_ShouldThrowFormatException_WhenBadTimeFormat()
    {
        // Arrange
        const string input = "archive=MyArchive\r\n" +
                             "size=1024\r\n" +
                             "time=NotADateTime\r\n";

        // Act/Assert
        Action act = () => PackedMetaData.ReadFromString(input);
        act.Should().Throw<FormatException>()
            .WithMessage("Expected DateTime value for header 'time='");
    }

    [Fact]
    public void ReadFromString_ShouldThrowFormatException_WhenNoModNames()
    {
        // Arrange
        const string input = "archive=MyArchive\r\n" +
                             "size=1024\r\n" +
                             "time=09/22/2023 02:04:31\r\n";

        // Act/Assert
        Action act = () => PackedMetaData.ReadFromString(input);
        act.Should().Throw<FormatException>()
            .WithMessage("Missing packed mod names");
    }
}
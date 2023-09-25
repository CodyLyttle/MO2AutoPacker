using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Tests.Helpers;

namespace MO2AutoPacker.Library.Tests;

public sealed class VirtualArchiveTests : IDisposable
{
    private readonly TemporaryDirectory _tempDir = new();

    public void Dispose() => _tempDir.Dispose();

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenCapacityLessThanOne(int capacity) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new VirtualArchive(capacity));

    [Fact]
    public void AddFile_ShouldThrowInvalidOperationException_WhenFileSizeExceedsMaximumArchiveSize()
    {
        // Arrange
        VirtualArchive testTarget = new(127);
        _tempDir.Root.AddFile(128, out string filePath);
        FileInfo file = new(filePath);

        // Assert
        Assert.Throws<InvalidOperationException>(() => testTarget.AddFile(file));
    }

    [Fact]
    public void AddFile_ShouldThrowInvalidOperationException_WhenFileSizeExceedsRemainingArchiveSize()
    {
        // Arrange
        VirtualArchive testTarget = new(128);
        TemporaryFolder folder = _tempDir.Root
            .AddFile(100)
            .AddFile(100);

        FileInfo[] files = folder.Directory.EnumerateFiles().ToArray();
        testTarget.AddFile(files[0]);

        // Assert
        Assert.Throws<InvalidOperationException>(() => testTarget.AddFile(files[1]));
    }

    [Fact]
    public void AddFile_ShouldThrowInvalidOperationException_WhenFileAlreadyExists()
    {
        // Arrange
        VirtualArchive testTarget = new(int.MaxValue);
        _tempDir.Root.AddFile(out string filePath);
        FileInfo file = new(filePath);
        testTarget.AddFile(file);

        // Assert
        Assert.Throws<InvalidOperationException>(() => testTarget.AddFile(file));
    }

    [Fact]
    public void AddFile_ShouldAddFileToArchive()
    {
        // Arrange
        VirtualArchive testTarget = new(int.MaxValue);
        _tempDir.Root.AddFile(out string filePath);
        FileInfo file = new(filePath);

        // Act
        testTarget.AddFile(file);

        // Assert
        Assert.Equal(filePath, testTarget.EnumerateFilePaths().First());
    }

    [Fact]
    public void AddFile_ShouldUpdateMetrics()
    {
        VirtualArchive testTarget = new(int.MaxValue);
        var fileCount = 0;
        long occupied = 0;
        long vacant = int.MaxValue;

        for (var i = 0; i < 10; i++)
        {
            _tempDir.Root.AddFile(out string filePath);
            FileInfo file = new(filePath);
            testTarget.AddFile(file);
            fileCount++;
            occupied += file.Length;
            vacant -= file.Length;

            Assert.Equal(fileCount, testTarget.FileCount);
            Assert.Equal(occupied, testTarget.OccupiedBytes);
            Assert.Equal(vacant, testTarget.VacantBytes);
        }
    }
}
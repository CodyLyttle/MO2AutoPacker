namespace MO2AutoPacker.Library.Models;

public class VirtualArchive
{
    private readonly HashSet<string> _filePaths = new();

    public VirtualArchive(long maxCapacityBytes)
    {
        if (maxCapacityBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCapacityBytes), maxCapacityBytes,
                "Must be greater than zero");

        MaxCapacityBytes = maxCapacityBytes;
        VacantBytes = MaxCapacityBytes;
    }

    public int FileCount { get; private set; }
    public long MaxCapacityBytes { get; }
    public long OccupiedBytes { get; private set; }
    public long VacantBytes { get; private set; }

    public void AddFile(FileInfo fileInfo)
    {
        string path = fileInfo.FullName;
        long fileSize = fileInfo.Length;

        if (fileInfo.Length > VacantBytes)
            throw new InvalidOperationException("File size exceeds remaining archive size");

        if (_filePaths.Contains(path))
            throw new InvalidOperationException($"Archive already contains file '{path}'");

        _filePaths.Add(path);
        FileCount++;
        OccupiedBytes += fileSize;
        VacantBytes -= fileSize;
    }

    public IEnumerable<string> EnumerateFilePaths() => _filePaths;
}
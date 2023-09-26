namespace MO2AutoPacker.Library.Tests.Helpers;

public class TemporaryFolder
{
    public TemporaryFolder(DirectoryInfo directory)
    {
        Directory = directory;
    }

    public string Path => Directory.FullName;

    public DirectoryInfo Directory { get; }

    private static string RandomName => Guid.NewGuid().ToString();

    private static int RandomFileSize => Random.Shared.Next(0, 256);

    private static byte[] RandomBuffer(int length)
    {
        var buffer = new byte[length];
        for (var i = 0; i < length; i++)
            buffer[i] = (byte) Random.Shared.Next(256);

        return buffer;
    }

    public TemporaryFolder AddFolder(string name) => new(Directory.CreateSubdirectory(name));

    public TemporaryFolder AddFile() => AddFile(RandomName);

    public TemporaryFolder AddFile(string name) => AddFile(name, RandomFileSize);

    public TemporaryFolder AddFile(int size) => AddFile(RandomName, size);

    public TemporaryFolder AddFile(string name, int size)
    {
        AddFileToFolder(name, size);
        return this;
    }

    public TemporaryFolder AddFile(out FileInfo filePath) => AddFile(RandomName, out filePath);

    public TemporaryFolder AddFile(string name, out FileInfo filePath) => AddFile(name, RandomFileSize, out filePath);

    public TemporaryFolder AddFile(int size, out FileInfo filePath) => AddFile(RandomName, size, out filePath);

    public TemporaryFolder AddFile(string name, int size, out FileInfo filePath)
    {
        filePath = AddFileToFolder(name, size);
        return this;
    }


    private FileInfo AddFileToFolder(string name, int size)
    {
        string filePath = System.IO.Path.Combine(Directory.FullName, name);
        FileStream stream = File.Create(filePath, size);
        stream.Write(RandomBuffer(size));
        stream.Close();

        return new FileInfo(filePath);
    }
}
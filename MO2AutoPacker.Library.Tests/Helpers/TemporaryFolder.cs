namespace MO2AutoPacker.Library.Tests.Helpers;

public class TemporaryFolder
{
    public string Path => Directory.FullName;
    public DirectoryInfo Directory { get; }

    public TemporaryFolder(DirectoryInfo directory)
    {
        Directory = directory;
    }
    
    public TemporaryFolder AddFolder(string name)
        => new(Directory.CreateSubdirectory(name));
    
    public TemporaryFolder AddFile() 
        => AddFile(GetRandomName());

    public TemporaryFolder AddFile(string name)
        => AddFile(name, GetRandomFileSize());

    public TemporaryFolder AddFile(int size)
        => AddFile(GetRandomName(), size);

    public TemporaryFolder AddFile(string name, int size)
    {
        AddFileToFolder(name, size);
        return this;
    }

    public TemporaryFolder AddFile(out string filePath)
        => AddFile(GetRandomName(), out filePath);

    public TemporaryFolder AddFile(string name, out string filePath)
        => AddFile(name, GetRandomFileSize(), out filePath);

    public TemporaryFolder AddFile(int size, out string filePath)
        => AddFile(GetRandomName(), size, out filePath);

    public TemporaryFolder AddFile(string name, int size, out string filePath)
    {
        filePath = AddFileToFolder(name, size);
        return this;
    }

    private string AddFileToFolder(string name, int size)
    {
        string filePath = System.IO.Path.Combine(Directory.FullName, name);
        FileStream stream = File.Create(filePath, size);
        stream.Write(GetRandomBuffer(size));
        stream.Close();

        return filePath;
    }

    private static string GetRandomName() => Guid.NewGuid().ToString();

    private static int GetRandomFileSize() => Random.Shared.Next(0, 256);

    private static byte[] GetRandomBuffer(int length)
    {
        var buffer = new byte[length];
        for (var i = 0; i < length; i++)
        {
            buffer[i] = (byte) Random.Shared.Next(256);
        }

        return buffer;
    }
}
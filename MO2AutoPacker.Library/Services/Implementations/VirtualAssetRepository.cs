using System.Runtime.CompilerServices;
using MO2AutoPacker.Library.Models;

[assembly: InternalsVisibleTo("MO2AutoPacker.Library.Tests")]
namespace MO2AutoPacker.Library.Services.Implementations;

// Keeps track of loose asset files and the mods that they belong to.
// Imitates MO2's virtual file system by overwriting conflicting asset paths.
internal class VirtualAssetRepository : IVirtualAssetRepository
{
    private readonly object _fileLock = new();

    // TODO: Add packable folders for lod, audio, etc...
    internal static readonly string[] AssetFolderNames =
    {
        "meshes",
        "textures",
    };

    // eg. Relative path -> ModPath
    // ["textures/textureA.dds"]= "C:/Mods/ModA/"
    private readonly Dictionary<string, string> _filePathRepository = new();
    
    public int FileCount { get; private set; }

    // Max Oblivion BSA size is 2GB.
    // Reserve a little space for the file header.
    public long ArchiveSizeInBytes { get; internal set; } = int.MaxValue - 1024;

    /// <summary>
    /// Used by test methods to verify internal state. 
    /// </summary>
    /// <returns>Pairs with Key: RelativeFilePath Value: ModDirectoryPath</returns>
    internal IEnumerable<KeyValuePair<string, string>> EnumerateFilePaths()
    {
        lock (_fileLock)
        {
            using IEnumerator<KeyValuePair<string, string>> enumerator = _filePathRepository.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }

    public void AddMod(Mod mod)
    {
        lock (_fileLock) // Prevent creation of archives while adding file paths.
        {
            DirectoryInfo modDir = new DirectoryInfo(mod.Path);
            if (!modDir.Exists)
                throw new FileNotFoundException("Missing mod directory", mod.Path);

            foreach (DirectoryInfo subDir in GetAssetDirectories(modDir))
            {
                foreach (string relativeFilePath in GetFilesRecursively(subDir, subDir.Name))
                {
                    _filePathRepository[relativeFilePath] = mod.Path;
                    FileCount++;
                }
            }
        }
    }

    private static IEnumerable<DirectoryInfo> GetAssetDirectories(DirectoryInfo modDir)
    {
        foreach (DirectoryInfo subDir in modDir.GetDirectories())
        {
            if (AssetFolderNames.Contains(subDir.Name, StringComparer.OrdinalIgnoreCase))
                yield return subDir;
        }
    }

    // TODO: Should we validate the file extension? .dds, .nif, etc...
    private static IEnumerable<string> GetFilesRecursively(DirectoryInfo currentDir, string relativeDir)
    {
        foreach (DirectoryInfo subDir in currentDir.GetDirectories())
        {
            string updatedRelativeDir = Path.Combine(relativeDir, subDir.Name);
            foreach (string relativeFilePath in GetFilesRecursively(subDir, updatedRelativeDir))
                yield return relativeFilePath;
        }

        foreach (FileInfo filePath in currentDir.GetFiles())
        {
            string relativeFilePath = Path.Combine(relativeDir, filePath.Name);
            yield return relativeFilePath;
        }
    }

    public IEnumerable<VirtualArchive> CreateVirtualArchives()
    {
        lock (_fileLock) // Prevent modification of file paths while creating archives.
        {
            using IEnumerator<KeyValuePair<string, string>> pathPairs = _filePathRepository.GetEnumerator();
            VirtualArchive archive = new(ArchiveSizeInBytes);

            while (pathPairs.MoveNext())
            {
                string filePath = Path.Combine(pathPairs.Current.Value, pathPairs.Current.Key);
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                    throw new FileNotFoundException("Missing asset file", filePath);

                long size = fileInfo.Length;
                if (size > ArchiveSizeInBytes)
                    throw new InvalidOperationException($"File exceeds the maximum BSA file size '{fileInfo.Name}'");

                if (size > archive.VacantBytes)
                {
                    yield return archive;
                    archive = new VirtualArchive(ArchiveSizeInBytes);
                }

                archive.AddFile(fileInfo);
            }

            if (archive.FileCount > 0)
                yield return archive;
        }
    }
}
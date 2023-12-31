﻿using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using MO2AutoPacker.Library.Logging;
using MO2AutoPacker.Library.Models;

[assembly: InternalsVisibleTo("MO2AutoPacker.Library.Tests")]

namespace MO2AutoPacker.Library.Services.Implementations;

// Keeps track of loose asset files and the mods that they belong to.
// Imitates MO2's virtual file system by overwriting conflicting assets.
internal class VirtualAssetRepository : IVirtualAssetRepository
{
    internal static readonly string[] AssetFolderNames =
    {
        "distantLOD",
        "meshes",
        "sound",
        "textures",
        "trees"
    };

    // eg. Relative path -> mod directory.
    // ["textures/textureA.dds"]= "C:/MO2/mods/SomeModName/"
    private readonly Dictionary<string, DirectoryInfo> _assetRepository = new();
    private readonly object _assetsLock = new();
    private readonly IDirectoryReader _directoryReader;

    public VirtualAssetRepository(IDirectoryReader directoryReader)
    {
        _directoryReader = directoryReader;
    }

    // Max Oblivion BSA size is 2GB.
    // Reserve a little space for the file header.
    public long ArchiveSizeInBytes { get; internal set; } = int.MaxValue - 1024;

    public int AddedFileCount { get; private set; }

    public int UniqueFileCount
    {
        get
        {
            lock (_assetsLock)
            {
                return _assetRepository.Count;
            }
        }
    }

    public void AddMod(Mod mod)
    {
        lock (_assetsLock)
        {
            Logger.Current.LogTrace("Adding mod '{ModName}'", mod.Name);

            DirectoryInfo modDir = _directoryReader.GetModFolder(mod.Name);
            IEnumerable<DirectoryInfo> assetDirectories = modDir.GetDirectories()
                .Where(dir => AssetFolderNames.Contains(dir.Name, StringComparer.InvariantCultureIgnoreCase));

            foreach (DirectoryInfo assetDir in assetDirectories)
            {
                foreach (string relativeAssetPath in GetAssetsRecursively(assetDir, assetDir.Name))
                {
                    _assetRepository[relativeAssetPath] = modDir;
                    AddedFileCount++;
                }
            }
        }
    }

    public IEnumerable<VirtualArchive> CreateVirtualArchives(CancellationToken? token = null)
    {
        lock (_assetsLock)
        {
            if (token is {IsCancellationRequested: true})
                throw new TaskCanceledException();

            VirtualArchive archive = new(ArchiveSizeInBytes);

            using Dictionary<string, DirectoryInfo>.Enumerator pathPairs = _assetRepository.GetEnumerator();
            while (pathPairs.MoveNext())
            {
                if (token is {IsCancellationRequested: true})
                    throw new TaskCanceledException();

                FileInfo fileInfo = GetFile(pathPairs.Current);
                long size = fileInfo.Length;
                if (size > ArchiveSizeInBytes)
                    throw new InvalidOperationException(
                        $"File exceeds the maximum BSA file size '{fileInfo.Name}'");

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

    /// <summary>
    ///     Used by test methods to verify internal state.
    /// </summary>
    /// <returns>A array of FileInfo objects representing the assets in the repository</returns>
    internal FileInfo[] GetAssetFiles()
    {
        lock (_assetsLock)
        {
            return _assetRepository
                .Select(GetFile)
                .ToArray();
        }
    }

    // TODO: Should we validate the file extension? .dds, .nif, etc...
    private static IEnumerable<string> GetAssetsRecursively(DirectoryInfo currentDir, string relativeDir)
    {
        foreach (DirectoryInfo subDir in currentDir.GetDirectories())
        {
            string updatedRelativeDir = Path.Combine(relativeDir, subDir.Name);
            foreach (string relativeFilePath in GetAssetsRecursively(subDir, updatedRelativeDir))
                yield return relativeFilePath;
        }

        foreach (FileInfo filePath in currentDir.GetFiles())
        {
            string relativeFilePath = Path.Combine(relativeDir, filePath.Name);
            yield return relativeFilePath;
        }
    }

    private static FileInfo GetFile(KeyValuePair<string, DirectoryInfo> pathPair)
    {
        FileInfo file = new(Path.Combine(pathPair.Value.FullName, pathPair.Key));
        if (!file.Exists)
            throw new FileNotFoundException(
                "A file from the virtual asset repository has been moved or deleted", file.FullName);

        return file;
    }
}
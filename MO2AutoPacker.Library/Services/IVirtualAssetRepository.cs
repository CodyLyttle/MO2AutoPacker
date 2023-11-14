using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Services;

public interface IVirtualAssetRepository
{
    int AddedFileCount { get; }
    int UniqueFileCount { get; }
    void AddMod(Mod mod);
    IEnumerable<VirtualArchive> CreateVirtualArchives(CancellationToken? token = null);
}
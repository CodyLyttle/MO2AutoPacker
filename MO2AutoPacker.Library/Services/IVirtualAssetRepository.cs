using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Services;

public interface IVirtualAssetRepository
{
    int FileCount { get; }
    void AddMod(Mod mod);
    IEnumerable<VirtualArchive> CreateVirtualArchives();
}
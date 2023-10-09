using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Services;

public interface IModListReader
{
    ModList Read(Profile profile);
}
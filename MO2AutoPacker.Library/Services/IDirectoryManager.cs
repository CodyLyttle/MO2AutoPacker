namespace MO2AutoPacker.Library.Services;

public interface IDirectoryManager : IDirectoryReader
{
    void SetModOrganizer(string path);
}
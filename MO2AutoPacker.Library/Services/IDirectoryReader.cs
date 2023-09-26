﻿namespace MO2AutoPacker.Library.Services;

public interface IDirectoryReader
{
    DirectoryInfo GetModOrganizer();
    DirectoryInfo GetModsFolder();
    DirectoryInfo GetModFolder(string modName);
    IEnumerable<DirectoryInfo> GetModFolders();
    DirectoryInfo GetProfilesFolder();
    IEnumerable<DirectoryInfo> GetProfileFolders();
}
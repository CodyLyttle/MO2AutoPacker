using System;
using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.ViewModels;
using MO2AutoPacker.UI.Implementations;

namespace MO2AutoPacker.UI;

public static class DesignMocks
{
    private static readonly IMessenger Messenger = new WeakReferenceMessenger();
    private static readonly IDirectoryManager DirectoryManager = new DesignDirectoryManager();
    private static readonly IUIThreadDispatcher Dispatcher = new WpfDispatcher();
    private static readonly IPathPicker PathPicker = new WindowsPathPicker();
    private static readonly IModListReader ModListReader = new DesignModListReader();

    public static readonly MainWindowViewModel MainWindow = new(Messenger, PathPicker, DirectoryManager);

    public static readonly BannerViewModel Banner = new(Messenger, Dispatcher);

    public static readonly ProfileSelectorViewModel ProfileSelector = new(Messenger, DirectoryManager);

    public static readonly ModListManagerViewModel ModListManager = new(Messenger, ModListReader);

    public static readonly Mod Mod = new("My Mod", true);

    public static readonly ModSeparator ModSeparator = new("~~Mod Separator~~");

    private class DesignDirectoryManager : IDirectoryManager
    {
        public DirectoryInfo GetArchiverFolder() => throw new NotImplementedException();
        public FileInfo GetArchiverExecutable() => throw new NotImplementedException();
        public DirectoryInfo GetModOrganizerFolder() => throw new NotImplementedException();
        public DirectoryInfo GetModsFolder() => throw new NotImplementedException();
        public DirectoryInfo GetModFolder(string modName) => throw new NotImplementedException();
        public IEnumerable<DirectoryInfo> GetModFolders() => throw new NotImplementedException();
        public DirectoryInfo GetProfilesFolder() => throw new NotImplementedException();
        public IEnumerable<DirectoryInfo> GetProfileFolders() => throw new NotImplementedException();
        public FileInfo GetModList(string profileName) => throw new NotImplementedException();

        public void SetModOrganizerFolder(string path) => throw new NotImplementedException();
        public void SetArchiverFolder(string path) => throw new NotImplementedException();
    }

    private class DesignModListReader : IModListReader
    {
        public ModList Read(Profile profile) =>
            new("Design Profile",
                new ModSeparator("Separator"),
                new Mod("Mod A", false),
                new Mod("Mod A", true));
    }
}
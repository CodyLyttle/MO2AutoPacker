using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;

namespace MO2AutoPacker.Library.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    private readonly IDirectoryManager _directoryManager;
    private readonly IMessenger _messenger;
    private readonly IModListReader _modListReader;
    private readonly IPathPicker _pathPicker;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPackArchive))]
    private string _archiverPath;

    // Nullable initializer prevents constructor warning when setting initial value via property.
    private string _modOrganizerPath = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPackArchive))]
    private Profile? _selectedProfile;

    public MainWindowViewModel(IMessenger messenger, IPathPicker pathPicker,
        IDirectoryManager directoryManager, IModListReader modListReader)
    {
        _messenger = messenger;
        _messenger.Register(this);
        _pathPicker = pathPicker;
        _directoryManager = directoryManager;
        _modListReader = modListReader;

        ArchiverPath = directoryManager.IsArchiverDirectoryInitialized
            ? directoryManager.GetArchiverFolder().FullName
            : string.Empty;

        ModOrganizerPath = directoryManager.IsModOrganizerDirectoryInitialized
            ? directoryManager.GetModOrganizerFolder().FullName
            : string.Empty;

        // TODO: Remove temporary workaround.
        // PackArchiveCommand.CanExecute isn't updating when ParkArchiveCommand changes.
        PropertyChanged += (_, args) =>
        {
            string name = args.PropertyName!;
            if (name is nameof(ModOrganizerPath) or nameof(ArchiverPath) or nameof(SelectedProfile))
                PackArchiveCommand.NotifyCanExecuteChanged();
        };
    }

    public string ModOrganizerPath
    {
        get => _modOrganizerPath;
        set
        {
            if (value == _modOrganizerPath)
                return;

            _modOrganizerPath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanPackArchive));
            _messenger.Send(new ModOrganizerPathChangedMessage());
        }
    }

    public bool CanPackArchive =>
        Path.Exists(ModOrganizerPath)
        && Path.Exists(ArchiverPath)
        && SelectedProfile is not null;

    public void Receive(ProfileChangedMessage message) => SelectedProfile = message.Profile;

    [RelayCommand]
    private void PickModOrganizerPath()
    {
        DirectoryInfo? newDir = _pathPicker.PickDirectory();

        // Picker cancelled/closed or identical path picked.
        if (newDir == null || newDir.FullName == ModOrganizerPath)
            return;

        try
        {
            _directoryManager.SetModOrganizerFolder(newDir.FullName);
            ModOrganizerPath = newDir.FullName;
        }
        catch (DirectoryNotFoundException ex)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, ex.Message));
        }
    }

    [RelayCommand]
    private void PickArchiverPath()
    {
        DirectoryInfo? newDir = _pathPicker.PickDirectory();

        if (newDir == null || newDir.FullName == ArchiverPath)
            return;

        try
        {
            _directoryManager.SetArchiverFolder(newDir.FullName);
            ArchiverPath = newDir.FullName;
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException or FileNotFoundException)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, ex.Message));
        }
    }

    [RelayCommand(CanExecute = nameof(CanPackArchive))]
    private void PackArchive()
    {
        Debug.WriteLine("Start packing archive");
        Stopwatch sw = new();
        sw.Start();

        ModList modList = _modListReader.Read(SelectedProfile!);
        VirtualAssetRepository virtualRepo = new(_directoryManager);
        foreach (Mod mod in modList.GetModsEnabled())
            virtualRepo.AddMod(mod);

        var count = 0;
        foreach (VirtualArchive arch in virtualRepo.CreateVirtualArchives())
            Debug.WriteLine($"Archive #{count++}: {arch.FileCount} files");

        sw.Stop();
        Debug.WriteLine($"Packed {count} archives in {sw.ElapsedMilliseconds}");
    }
}
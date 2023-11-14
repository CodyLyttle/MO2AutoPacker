using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MO2AutoPacker.Library.Logging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;

namespace MO2AutoPacker.Library.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    private readonly IConfirmationDialog _confirmationDialog;
    private readonly IDirectoryManager _directoryManager;
    private readonly IMessenger _messenger;
    private readonly IModListReader _modListReader;
    private readonly IPathPicker _pathPicker;
    private readonly IUIThreadDispatcher _dispatcher;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPackArchive))]
    private string _archiverPath;

    // Nullable initializer prevents constructor warning when setting initial value via property.
    private string _modOrganizerPath = null!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanPackArchive))]
    private Profile? _selectedProfile;

    [ObservableProperty]
    private bool _isPackingArchives;

    public MainWindowViewModel(IMessenger messenger, IConfirmationDialog confirmationDialog, IPathPicker pathPicker,
        IDirectoryManager directoryManager, IModListReader modListReader, IUIThreadDispatcher dispatcher)
    {
        _messenger = messenger;
        _messenger.Register(this);
        _confirmationDialog = confirmationDialog;
        _pathPicker = pathPicker;
        _dispatcher = dispatcher;
        _directoryManager = directoryManager;
        _modListReader = modListReader;

        ArchiverPath = directoryManager.IsArchiverDirectoryInitialized
            ? directoryManager.GetArchiverFolder().FullName
            : string.Empty;

        ModOrganizerPath = directoryManager.IsModOrganizerDirectoryInitialized
            ? directoryManager.GetModOrganizerFolder().FullName
            : string.Empty;

        PackModsCommand = new CancellableCommand(dispatcher, PackArchive)
        {
            CanBegin = CanPackArchive
        };

        PackModsCommand.Completed += (_, success) =>
        {
            if (!success)
            {
                PackModsCommand.ResetFailedCommand();
                Logger.Current.LogInformation("Cancelled archive task");
            }
        };

        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(CanPackArchive))
                PackModsCommand.CanBegin = CanPackArchive;
        };
    }

    public CancellableCommand PackModsCommand { get; }

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

    private bool CanPackArchive =>
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

    private Task PackArchive(CancellationToken token)
    {
        ThrowIfCancelled();

        ModList modList = _modListReader.Read(SelectedProfile!);
        VirtualAssetRepository virtualRepo = new(_directoryManager);
        var packedArchiveCount = 0;
        var fileSum = 0;

        Logger.Current.LogInformation("Creating virtual archives for profile '{ProfileName}'",
            SelectedProfile!.Name);

        var sw = Stopwatch.StartNew();
        foreach (Mod mod in modList.GetModsEnabled())
        {
            ThrowIfCancelled();
            virtualRepo.AddMod(mod);
        }

        foreach (VirtualArchive arch in virtualRepo.CreateVirtualArchives())
        {
            ThrowIfCancelled();
            int fileCount = arch.FileCount;
            fileSum += fileCount;

            Logger.Current.LogInformation("Archive #{ArchiveIndex}: {FileCount} files",
                packedArchiveCount++, fileCount);
        }

        sw.Stop();
        Logger.Current.LogInformation("Packed {ArchiveCount} archives in {ElapsedMS}ms",
            packedArchiveCount, sw.ElapsedMilliseconds);

        ThrowIfCancelled();
        
        // Disable UI until dialog closed.
        _dispatcher.Invoke(() => IsEnabled = false);
        
        // Prompt user before creating real archives.
        _confirmationDialog.PromptUser($"Archive profile '{SelectedProfile?.Name}'",
            $"Create {packedArchiveCount} archives from {fileSum} files?");
        
        _dispatcher.Invoke(() => IsEnabled = true);
        
        return Task.CompletedTask;

        void ThrowIfCancelled()
        {
            if (token.IsCancellationRequested)
                throw new OperationCanceledException(token);
        }
    }
}
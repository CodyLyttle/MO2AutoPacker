using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.ViewModels;
using MO2AutoPacker.UI.Implementations;

namespace MO2AutoPacker.UI;

public static class DesignMocks
{
    private static readonly IMessenger Messenger = new WeakReferenceMessenger();
    private static readonly IUIThreadDispatcher Dispatcher = new WpfDispatcher();

    public static readonly MainWindowViewModel MainWindow = new(Messenger);

    public static readonly BannerViewModel Banner = new(Messenger, Dispatcher);

    public static readonly PathPickerViewModel PathPicker = new(Messenger, PathKey.ModOrganizerRoot, "Watermark");

    public static readonly ProfileSelectorViewModel ProfileSelector = new(Messenger);

    public static readonly ModListManagerViewModel ModListManager = new(Messenger);
}
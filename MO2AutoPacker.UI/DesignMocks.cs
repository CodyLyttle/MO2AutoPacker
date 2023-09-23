using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.UIAbstractions;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.ViewModels;
using MO2AutoPacker.UI.Implementations;

namespace MO2AutoPacker.UI;

public static class DesignMocks
{
    private static readonly IMessenger MessengerDependency = new WeakReferenceMessenger();
    private static readonly IUIThreadDispatcher Dispatcher = new WpfDispatcher();

    public static readonly MainWindowViewModel MainWindow = new(MessengerDependency);

    public static readonly BannerViewModel Banner = new(MessengerDependency, Dispatcher);

    public static readonly PathPickerViewModel PathPicker = new(MessengerDependency, PathKey.ModOrganizerRoot, "Watermark");

    public static readonly ProfileSelectorViewModel ProfileSelector = new(MessengerDependency);
}
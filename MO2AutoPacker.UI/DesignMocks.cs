using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.UI.ViewModels;

namespace MO2AutoPacker.UI;

public static class DesignMocks
{
    private static readonly IMessenger MessengerDependency = new WeakReferenceMessenger();

    public static readonly MainWindowViewModel MainWindow = new(MessengerDependency);

    public static readonly BannerViewModel Banner = new(MessengerDependency);

    public static readonly PathPickerViewModel PathPicker = new(MessengerDependency, "Watermark");
}
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.Library;

/// <summary>
///     Basic dependency bootstrapper.
/// </summary>
// Subject to change if further features are required, such as swapping dependencies at run-time.
public static class Bootstrapper
{
    public static IServiceProvider CreateServiceProvider(IConfirmationDialog dialog,
        IUIThreadDispatcher dispatcher,
        IPathPicker pathPicker)
    {
        IServiceCollection services = new ServiceCollection()
            // Services
            .AddSingleton(dialog)
            .AddSingleton(dispatcher)
            .AddSingleton(pathPicker)
            .AddSingleton<IPathReader, PathReader>()
            .AddSingleton<DirectoryManager>()
            .AddSingleton<IDirectoryManager>(x => x.GetRequiredService<DirectoryManager>())
            .AddSingleton<IDirectoryReader>(x => x.GetRequiredService<DirectoryManager>())
            .AddSingleton<IModListReader, ModListReader>()
            .AddSingleton<IVirtualAssetRepository, VirtualAssetRepository>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            // View models.
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BannerViewModel>()
            .AddSingleton<ProfileSelectorViewModel>()
            .AddSingleton<ModListManagerViewModel>();

        return services.BuildServiceProvider();
    }
}
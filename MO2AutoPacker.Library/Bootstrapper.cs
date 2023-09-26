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
    public static IServiceProvider CreateServiceProvider(IUIThreadDispatcher dispatcher, IPathPicker pathPicker) =>
        new ServiceCollection()
            // Services
            .AddSingleton(dispatcher)
            .AddSingleton(pathPicker)
            .AddSingleton<DirectoryManager>()
            .AddSingleton<IDirectoryManager>(x => x.GetRequiredService<DirectoryManager>())
            .AddSingleton<IDirectoryReader>(x => x.GetRequiredService<DirectoryManager>())
            .AddSingleton<IVirtualAssetRepository, VirtualAssetRepository>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            // View models.
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BannerViewModel>()
            .AddSingleton<ProfileSelectorViewModel>()
            .AddSingleton<ModListManagerViewModel>()
            .BuildServiceProvider();
}
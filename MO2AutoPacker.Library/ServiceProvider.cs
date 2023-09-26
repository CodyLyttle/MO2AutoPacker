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
public class ServiceProvider
{
    private static IServiceProvider? _services;

    public static T GetService<T>() where T : notnull
    {
        if (_services == null)
            throw new InvalidOperationException(
                "Attempted to retrieve a service before initializing the service provider");

        return _services.GetRequiredService<T>();
    }

    public static void Initialize(IUIThreadDispatcher dispatcher)
    {
        if (_services != null)
            throw new InvalidOperationException("Services have already been initialized");

        IServiceCollection collection = new ServiceCollection()
            // Services
            .AddSingleton(dispatcher)
            .AddSingleton<DirectoryManager>()
            .AddSingleton<IDirectoryManager>(x => x.GetRequiredService<DirectoryManager>())
            .AddSingleton<IDirectoryReader>(x => x.GetRequiredService<DirectoryManager>())
            .AddSingleton<IVirtualAssetRepository, VirtualAssetRepository>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            // View models.
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BannerViewModel>()
            .AddSingleton<ModListManagerViewModel>();

        _services = collection.BuildServiceProvider();
    }
}
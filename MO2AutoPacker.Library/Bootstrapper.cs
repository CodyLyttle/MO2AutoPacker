using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MO2AutoPacker.Library.Logging;
using MO2AutoPacker.Library.Services;
using MO2AutoPacker.Library.Services.Implementations;
using MO2AutoPacker.Library.ViewModels;
using Serilog;

namespace MO2AutoPacker.Library;

/// <summary>
///     Basic dependency bootstrapper.
/// </summary>
// Subject to change if further features are required, such as swapping dependencies at run-time.
public static class Bootstrapper
{
    public static IServiceProvider Bootstrap(IConfirmationDialog dialog,
        IUIThreadDispatcher dispatcher,
        IPathPicker pathPicker)
    {
        IServiceProvider serviceProvider = new ServiceCollection()
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
            .AddSingleton<ModListManagerViewModel>()
            .BuildServiceProvider();

        InitAmbientContext(serviceProvider);

        return serviceProvider;
    }

    private static void InitAmbientContext(IServiceProvider serviceProvider)
    {
        var messenger = serviceProvider.GetService<IMessenger>();
        Debug.Assert(messenger is not null);

        Logger.Current = LoggerFactory.Create(builder => builder
                .AddProvider(new MessengerLoggerProvider(messenger))
                .AddSerilog(new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Debug()
                    .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger()))
            .CreateLogger(nameof(MO2AutoPacker));
    }
}
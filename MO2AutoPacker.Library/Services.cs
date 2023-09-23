using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MO2AutoPacker.Library.Abstractions.UI;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.Library;

/// <summary>
/// Basic dependency bootstrapper.
/// </summary>
// Subject to change if further features are required, such as swapping dependencies at run-time.
public class Services
{
    private static IServiceProvider? _provider;

    public static IServiceProvider Provider
    {
        get
        {
            if (_provider == null)
                throw new InvalidOperationException(
                    $"The service provider is uninitialized");

            return _provider;
        }
    }
    
    public static void Initialize(IUIThreadDispatcher dispatcher)
    {
        if (_provider != null)
            throw new InvalidOperationException("Services have already been initialized");

        IServiceCollection collection = new ServiceCollection()
            .AddSingleton(dispatcher)
            .AddSingleton<IMessenger>(new WeakReferenceMessenger())
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BannerViewModel>()
            .AddSingleton<ModListManagerViewModel>();
        
        _provider = collection.BuildServiceProvider();
    }
}
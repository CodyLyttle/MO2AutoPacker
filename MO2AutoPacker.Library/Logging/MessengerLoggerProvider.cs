using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace MO2AutoPacker.Library.Logging;

public sealed class MessengerLoggerProvider : ILoggerProvider
{
    private readonly IMessenger _messenger;

    public MessengerLoggerProvider(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public void Dispose()
    {
        // Do nothing
    }

    public ILogger CreateLogger(string categoryName) => new MessengerLogger(_messenger);
}
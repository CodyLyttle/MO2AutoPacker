using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MO2AutoPacker.Library.Messages;

namespace MO2AutoPacker.Library.Logging;

public class MessengerLogger : ILogger
{
    private readonly IMessenger _messenger;

    public MessengerLogger(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        string time = DateTime.Now.ToShortTimeString();
        string formattedMessage = $"[{time}] {formatter(state, exception)}";
        _messenger.Send(new LogMessage(formattedMessage));
    }

    public bool IsEnabled(LogLevel logLevel) => (int) logLevel >= (int) LogLevel.Information;

    // We aren't using scoped logging at this time.
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
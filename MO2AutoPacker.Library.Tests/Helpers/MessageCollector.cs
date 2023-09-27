using CommunityToolkit.Mvvm.Messaging;

namespace MO2AutoPacker.Library.Tests.Helpers;

// Useful when dealing with multiple message types.
internal sealed class MessageCollector
{
    private readonly Queue<object> _messages = new();
    private readonly IMessenger _messenger;

    public MessageCollector(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public MessageCollector AddWhitelist<T>() where T : class
    {
        _messenger.Register<MessageCollector, T>(this, (_, message) => Receive(message));
        return this;
    }

    public MessageCollector AddBlacklist<T>() where T : class
    {
        _messenger.Register<MessageCollector, T>(this, (_, message) => throw new InvalidOperationException(
            $"Blacklisted message '{message.GetType().Name}'"));

        return this;
    }

    private void Receive<TMessage>(TMessage message)
    {
        if (message != null)
            _messages.Enqueue(message);
    }

    public T DequeueMessage<T>()
    {
        Assert.NotEmpty(_messages);
        Assert.IsType<T>(_messages.Peek());
        return (T) _messages.Dequeue();
    }

    public void AssertEmpty() => Assert.Empty(_messages);

    public void AssertCount<T>(int expected) where T : class => Assert.Equal(expected, _messages.OfType<T>().Count());
}
using CommunityToolkit.Mvvm.Messaging;

namespace MO2AutoPacker.Library.Tests.Stubs;

internal sealed class RecipientStub<T> : IRecipient<T> where T : class
{
    public RecipientStub(IMessenger messenger)
    {
        messenger.Register(this);
    }

    public Queue<T> Messages { get; } = new();

    public void Receive(T message) => Messages.Enqueue(message);
}
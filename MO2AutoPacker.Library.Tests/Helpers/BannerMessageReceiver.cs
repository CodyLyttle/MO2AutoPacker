using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;

namespace MO2AutoPacker.Library.Tests.Helpers;

internal sealed class BannerMessageReceiver : IRecipient<BannerMessage>
{
    public BannerMessageReceiver(IMessenger messenger)
    {
        messenger.Register(this);
    }

    public Queue<BannerMessage> Messages { get; } = new();

    public void Receive(BannerMessage message) => Messages.Enqueue(message);
}
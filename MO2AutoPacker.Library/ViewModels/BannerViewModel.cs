using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.UIAbstractions;

namespace MO2AutoPacker.Library.ViewModels;

public partial class BannerViewModel : ViewModelBase, IRecipient<BannerMessage>
{
    private readonly IUIThreadDispatcher _dispatcher;
    private readonly Queue<BannerMessage> _messageQueue = new();
    private bool _isTransitioning;

    [ObservableProperty]
    private bool _showMessage;

    [ObservableProperty]
    private BannerMessage? _message;

    public int QueuedMessageCount => _messageQueue.Count;

    // Public setter in-case we allow user to tweak animation speed in the future.
    public TimeSpan FadeDuration { get; set; } = TimeSpan.FromSeconds(0.2);

    public BannerViewModel(IMessenger messenger, IUIThreadDispatcher dispatcher)
    {
        messenger.Register(this);
        _dispatcher = dispatcher;
    }

    public void Receive(BannerMessage message)
    {
        _messageQueue.Enqueue(message);
        OnPropertyChanged(nameof(QueuedMessageCount));

        // Only display the message if the banner is currently empty.
        // Queued messages will be displayed when the current message is closed.
        if (Message == null)
            TransitionNextMessage();
    }

    [RelayCommand]
    private void TransitionNextMessage()
    {
        // Ignore requests to transition if transition already in progress.
        if (_isTransitioning)
            return;

        if (Message == null)
        {
            if (QueuedMessageCount > 0)
                FadeMessageIn();
        }
        else
        {
            FadeMessageOut();
        }
    }

    private void FadeMessageIn()
    {
        _isTransitioning = true;
        ShowMessage = true;
        Message = _messageQueue.Dequeue();
        OnPropertyChanged(nameof(QueuedMessageCount));

        Task.Run(async () =>
        {
            await Task.Delay(FadeDuration);
            _isTransitioning = false;
        });
    }

    private void FadeMessageOut()
    {
        _isTransitioning = true;
        ShowMessage = false;
        Task.Run(async () =>
        {
            await Task.Delay(FadeDuration);
            _isTransitioning = false;

            _dispatcher.Invoke(() => { Message = null; });
            Message = null;
            TransitionNextMessage();
        });
    }
}
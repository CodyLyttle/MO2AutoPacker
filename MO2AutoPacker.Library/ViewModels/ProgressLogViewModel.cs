using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ProgressLogViewModel : ViewModelBase, IRecipient<LogMessage>
{
    [ObservableProperty]
    private string _logContent = string.Empty; 
    
    public ProgressLogViewModel(IMessenger messenger)
    {
        messenger.Register(this);
    }

    public void Receive(LogMessage message)
    {
        LogContent += "\r\n" + message.Message;
    }
}
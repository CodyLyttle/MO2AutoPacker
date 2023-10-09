using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Models;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.ViewModels;

public partial class ModListManagerViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    private readonly IMessenger _messenger;
    private readonly IModListReader _modListReader;

    [ObservableProperty]
    private ModList? _modList;

    public ModListManagerViewModel(IMessenger messenger, IModListReader modListReader)
    {
        _messenger = messenger;
        _messenger.Register(this);
        _modListReader = modListReader;
    }

    public void Receive(ProfileChangedMessage message)
    {
        try
        {
            ModList = message.Profile is null
                ? null
                : _modListReader.Read(message.Profile);
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException or FileNotFoundException)
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, ex.Message));
        }
    }
}
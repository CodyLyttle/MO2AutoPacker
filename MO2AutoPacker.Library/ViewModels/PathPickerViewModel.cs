using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.Library.Messages;
using MO2AutoPacker.Library.Validation;

namespace MO2AutoPacker.Library.ViewModels;

public partial class PathPickerViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;
    private readonly List<Validators.Path> _pathValidators;

    [ObservableProperty]
    private string? _path;

    public PathKey Key { get; }
    public string Watermark { get; }

    public PathPickerViewModel(IMessenger messenger, PathKey key, string watermark)
    {
        _messenger = messenger;
        Key = key;
        Watermark = watermark;
        _pathValidators = new List<Validators.Path> {PrimaryPathValidator};
    }

    public void AddValidator(Validators.Path validator) => _pathValidators.Add(validator);

    [RelayCommand]
    private void UpdatePath(string? path)
    {
        if (path == null)
            return;

        ValidatorResult result = ValidatePath(path);
        if(result.WasSuccessful)
        {
            Path = path;
            _messenger.Send(new PathChangedMessage(Key, Path));
        }
        else
        {
            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, result.Message));
        }
    }

    private ValidatorResult ValidatePath(string path)
    {
        foreach (Validators.Path validator in _pathValidators)
        {
            ValidatorResult result = validator.Invoke(path);
            if (!result.WasSuccessful)
                return result;
        }
        
        return ValidatorResult.Success();
    }

    private static ValidatorResult PrimaryPathValidator(string path)
    {
        return Directory.Exists(path)
            ? ValidatorResult.Success()
            : ValidatorResult.Fail($"Directory '{path}' doesn't exist");
    }
}
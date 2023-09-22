using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.UI.Messages;

namespace MO2AutoPacker.UI.ViewModels;

public partial class PathPickerViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private string? _path;
    public string Watermark { get; }

    public PathPickerViewModel(IMessenger messenger, string watermark)
    {
        _messenger = messenger;
        Watermark = watermark;
    }

    [RelayCommand]
    private void UpdatePath(string? path)
    {
        if (path == null)
            return;

        if (ValidatePath(path, out string? errorMsg))
        {
            Path = path;
        }
        else
        {
            if (errorMsg == null)
                throw new InvalidOperationException("Error message must be provided for an invalid path");

            _messenger.Send(new BannerMessage(BannerMessage.Type.Error, errorMsg));
        }
    }

    private bool ValidatePath(string path, out string? errorMsg)
    {
        errorMsg = null;

        if (Directory.Exists(path))
            return DoValidatePath(path, out errorMsg);

        errorMsg = $"Directory '{path}' doesn't exist";
        return false;
    }

    /// <summary>
    /// A template method for appending extra logic to path validation. <br/>
    /// By default, a path is considered valid if the directory exists.
    /// </summary>
    /// <param name="path">The path to validate.</param>
    /// <param name="errorMsg">The error message holder.</param>
    /// <returns>True if the path is valid, otherwise false.</returns>
    protected virtual bool DoValidatePath(string path, out string? errorMsg)
    {
        errorMsg = null;
        return true;
    }
}
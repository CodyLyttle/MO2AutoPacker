﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using MO2AutoPacker.UI.Messages;
using MO2AutoPacker.UI.Validation;

namespace MO2AutoPacker.UI.ViewModels;

// The primary class responsible for managing application state.
// Any dependencies should be configured in App.xaml.cs and injected here.
public partial class MainWindowViewModel : ViewModelBase, IRecipient<ProfileChangedMessage>
{
    public PathPickerViewModel RootPathPicker { get; }
    
    public List<string> ProfilePaths { get; private set; } = new();

    public MainWindowViewModel(IMessenger messenger)
    {
        messenger.Register(this);
        RootPathPicker = new PathPickerViewModel(messenger, PathKey.ModOrganizerRoot, "Mod Organizer 2 path");
        RootPathPicker.AddValidator(ModOrganizerPathValidator);
    }

    // Delegate for RootPathPicker.
    private static ValidatorResult ModOrganizerPathValidator(string path)
    {
        List<string> pendingFolders = new()
        {
            "mods",
            "profiles"
        };

        foreach (string subDirectory in Directory.EnumerateDirectories(path))
        {
            string folder = Path.GetFileName(subDirectory)!;
            if (pendingFolders.Contains(folder))
            {
                pendingFolders.Remove(folder);
                if (pendingFolders.Count == 0)
                    return ValidatorResult.Success();
            }
        }

        StringBuilder builder = new("Invalid MO2 path - missing subdirectories");
        foreach (string subDir in pendingFolders)
        {
            builder.Append($" '{subDir}',");
        }

        // Remove the unnecessary comma.
        builder.Remove(builder.Length - 1, 1);

        return ValidatorResult.Fail(builder.ToString());
    }

    public void Receive(ProfileChangedMessage message)
    {
        // TODO: Update UI to reflect current profile.
    }
}
using System;
using System.IO;
using System.Windows.Forms;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.UI.Implementations;

public class WindowsPathPicker : IPathPicker
{
    public DirectoryInfo? PickDirectory() => GetDirectoryFromDialog(null);

    public DirectoryInfo? PickDirectory(DirectoryInfo initialDirectory) => GetDirectoryFromDialog(initialDirectory);

    public FileInfo? PickFile(params string[] extensionWhitelist) => GetFileFromDialog(null);

    public FileInfo? PickFile(DirectoryInfo initialDirectory, params string[] extensionWhitelist) =>
        GetFileFromDialog(initialDirectory);

    private static DirectoryInfo? GetDirectoryFromDialog(DirectoryInfo? initialDirectory)
    {
        using FolderBrowserDialog dialog = new();
        if (initialDirectory is not null)
            dialog.InitialDirectory = initialDirectory.FullName;

        return dialog.ShowDialog() == DialogResult.OK
            ? new DirectoryInfo(dialog.SelectedPath)
            : null;
    }

    // TODO: Implement if/when we require individual file selection.
    private static FileInfo? GetFileFromDialog(DirectoryInfo? initialDirectory) =>
        throw new NotImplementedException("File picker is not currently implemented");
}
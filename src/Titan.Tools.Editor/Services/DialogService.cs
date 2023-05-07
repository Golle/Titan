using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Titan.Tools.Editor.ViewModels;
using Titan.Tools.Editor.Views;

namespace Titan.Tools.Editor.Services;

internal class DialogService : IDialogService
{
    public async Task<NewProjectResult?> OpenNewProjectDialog(Window? parent)
    {
        var dialog = new NewProject();
        var parentWindow = parent ?? App.GetMainWindow();
        return await dialog.ShowDialog<NewProjectResult?>(parentWindow);
    }

    public async Task<SelectProjectResult?> OpenSelectProjectDialog(Window? parent = null)
    {
        var dialog = new SelectProjectWindow();
        var parentWindow = parent ?? App.GetMainWindow();
        return await dialog.ShowDialog<SelectProjectResult>(parentWindow);
    }

    public async Task<string?> OpenFileDialog(IReadOnlyList<FilePickerFileType>? fileTypes, Window? parent)
    {
        var window = parent ?? App.GetMainWindow();
        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = fileTypes,
            Title = "Open Titan Project"
        });
        if (result.Count > 0)
        {
            return result[0].Path.AbsolutePath;
        }
        return null;
    }
}

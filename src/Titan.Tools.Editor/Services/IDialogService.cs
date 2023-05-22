using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Titan.Tools.Editor.ViewModels;
using Titan.Tools.Editor.ViewModels.Dialogs;

namespace Titan.Tools.Editor.Services;

public interface IDialogService
{
    Task<NewProjectResult?> OpenNewProjectDialog(Window? parent = null);
    Task<SelectProjectResult?> OpenSelectProjectDialog(Window? parent = null);
    Task OpenProjectSettingsDialog(Window? parent = null);
    Task<string?> OpenFileDialog(IReadOnlyList<FilePickerFileType>? fileTypes = null, Window? parent = null);
    Task<MessageBoxResult?> ShowMessageBox(string title, string? message = null, MessageBoxType type = MessageBoxType.Ok, Window? parent = null);
}

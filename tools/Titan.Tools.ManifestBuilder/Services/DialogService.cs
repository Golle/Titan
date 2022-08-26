using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.Services;

public interface IDialogService
{
    Task<string?> OpenFileDialog(string? path = null);
    Task<string?> OpenFolderDialog(string? path = null);
    Task<MessageBoxResult?> MessageBox(string title, string? message = null, MessageBoxType type = MessageBoxType.Ok);
}

internal class DialogService : IDialogService
{
    public async Task<string?> OpenFileDialog(string? path)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Directory = path ?? Directory.GetCurrentDirectory(),
            Filters = new()
            {
                new FileDialogFilter
                {
                    Name = "Titan Manifest",
                    Extensions = { GlobalConfiguration.ManifestFileExtension }
                }
            }
        };

        var files = await dialog.ShowAsync(App.MainWindow);
        return files?.FirstOrDefault();
    }

    public async Task<string?> OpenFolderDialog(string? path)
    {
        var dialog = new OpenFolderDialog
        {
            Directory = path ?? Directory.GetCurrentDirectory(),
            Title = "Select folder"
        };
        return await dialog.ShowAsync(App.MainWindow);
    }

    public async Task<MessageBoxResult?> MessageBox(string title, string? message = null, MessageBoxType type = MessageBoxType.Ok)
        => await MessageBoxDialog.Create(title, message, type).ShowDialog<MessageBoxResult>(App.MainWindow);
}

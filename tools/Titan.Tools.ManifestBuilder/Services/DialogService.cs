using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;
using Titan.Tools.ManifestBuilder.Views;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.Services;

public interface IDialogService
{
    Task<string?> OpenFileDialog();
    Task<MessageBoxResult?> MessageBox(string title, string? message = null, MessageBoxType type = MessageBoxType.Ok);
}

internal class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string?> OpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Directory = Directory.GetCurrentDirectory(),
            Filters = new()
            {
                new FileDialogFilter
                {
                    Name = "Titan Manifest",
                    Extensions = { GlobalConfiguration.ManifestFileExtension }
                }
            }
        };

        var files = await dialog.ShowAsync(_serviceProvider.GetRequiredService<MainWindow>());
        return files?.FirstOrDefault();
    }

    public async Task<MessageBoxResult?> MessageBox(string title, string? message = null, MessageBoxType type = MessageBoxType.Ok) 
        => await MessageBoxDialog.Create(title, message, type).ShowDialog<MessageBoxResult>(App.MainWindow);
}

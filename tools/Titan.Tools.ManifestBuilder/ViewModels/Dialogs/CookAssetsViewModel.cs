using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

public class CookAssetsViewModel : ViewModelBase
{
    private string? _packagePath;
    private string? _generatedPath;
    private string? _namespace;

    public string? PackagePath
    {
        get => _packagePath;
        private set => SetProperty(ref _packagePath, value);
    }

    public string? GeneratedPath
    {
        get => _generatedPath;
        set => SetProperty(ref _generatedPath, value);
    }
    public string? Namespace
    {
        get => _namespace;
        set => SetProperty(ref _namespace, value);
    }

    public ICommand BrowsePackagePath { get; }
    public ICommand BrowseGeneratedPath { get; }
    public ICommand Build { get; }
    public CookAssetsViewModel(Window window, IDialogService? dialogService = null, IAppSettings? appSettings = null, IApplicationState? applicationState = null)
    {
        dialogService ??= Registry.GetRequiredService<IDialogService>();
        appSettings ??= Registry.GetRequiredService<IAppSettings>();
        applicationState ??= Registry.GetRequiredService<IApplicationState>();

        var cookSettings = appSettings.GetSettings().CookAssetSettings;
        _packagePath = cookSettings.OutputPath;
        _generatedPath = cookSettings.GeneratedPath;
        _namespace = cookSettings.Namespace;

        BrowseGeneratedPath = ReactiveCommand.CreateFromTask(async () => GeneratedPath = await dialogService.OpenFolderDialog(_generatedPath));
        BrowsePackagePath = ReactiveCommand.CreateFromTask(async () => PackagePath = await dialogService.OpenFolderDialog(_packagePath));
        Build = ReactiveCommand.CreateFromTask(async () =>
        {
            var settings = appSettings.GetSettings();
            appSettings.Save(settings with { CookAssetSettings = new CookAssetSettings(_namespace, PackagePath, GeneratedPath) });
            var projectPath = applicationState.ProjectPath;

            var manifests = Directory.GetFiles(projectPath!, $"*.{GlobalConfiguration.ManifestFileExtension}");
            if (manifests.Length == 0)
            {
                await dialogService.MessageBox("Failed", $"No manifests found in path {projectPath}");
                return;
            }

            var args = $"run --project {GlobalConfiguration.PackagerProjectPath} -- package {string.Join(' ', manifests.Select(m => $"-m {m}"))} -o \"{_packagePath}\" -g \"{_generatedPath}\" {(_namespace != null ? $"-n {_namespace}" : string.Empty)}";
            var dialog = new ExternalProcessWindow
            {
                DataContext = new ExternalProcessViewModel(new ExternalProcess("dotnet", args))
            };
            await dialog.ShowDialog(window);
            window.Close();
        });
    }

    public CookAssetsViewModel()
    : this(null!)
    {
    }
}



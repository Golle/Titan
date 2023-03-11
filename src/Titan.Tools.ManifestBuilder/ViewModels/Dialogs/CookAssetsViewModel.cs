using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

public class CookAssetsViewModel : ViewModelBase
{
    private string? _outputPath;
    private string? _generatedPath;
    private string? _namespace;
    private int? _manifestStartId;

    public string? OutputPath
    {
        get => _outputPath;
        private set => SetProperty(ref _outputPath, value);
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

    public int? ManifestStartId
    {
        get => _manifestStartId;
        set => SetProperty(ref _manifestStartId, value);
    }

    public ICommand BrowsePackagePath { get; }
    public ICommand BrowseGeneratedPath { get; }
    public ICommand Build { get; }
    public CookAssetsViewModel(Window window, IDialogService? dialogService = null, IApplicationState? applicationState = null)
    {
        dialogService ??= Registry.GetRequiredService<IDialogService>();
        applicationState ??= Registry.GetRequiredService<IApplicationState>();

        var projectSettings = applicationState.ProjectSettings;
        _outputPath = projectSettings.OutputPath;
        _generatedPath = projectSettings.GeneratedPath;
        _namespace = projectSettings.Namespace;
        _manifestStartId = projectSettings.ManifestStartId;

        BrowseGeneratedPath = ReactiveCommand.CreateFromTask(async () => GeneratedPath = await dialogService.OpenFolderDialog(_generatedPath));
        BrowsePackagePath = ReactiveCommand.CreateFromTask(async () => OutputPath = await dialogService.OpenFolderDialog(_outputPath));
        Build = ReactiveCommand.CreateFromTask(async () =>
        {
            var settings = applicationState.ProjectSettings;
            await applicationState.SaveAsync(settings with { GeneratedPath = GeneratedPath, OutputPath = OutputPath, Namespace = Namespace, ManifestStartId = ManifestStartId });
            var projectPath = applicationState.ProjectPath;

            var manifests = Directory.GetFiles(projectPath!, $"*.{GlobalConfiguration.ManifestFileExtension}");
            if (manifests.Length == 0)
            {
                await dialogService.MessageBox("Failed", $"No manifests found in path {projectPath}");
                return;
            }

            var (executable, extraArgs) = GlobalConfiguration.PackagerExecutablePath != null
                ? (GlobalConfiguration.PackagerExecutablePath, string.Empty)
                : ("dotnet", $"run --project {GlobalConfiguration.PackagerProjectPath} -- ");

            var libPath = $"-l \"{GlobalConfiguration.DxcLibPath}\"";
            var args = $"package {libPath} {string.Join(' ', manifests.Select(m => $"-m {m}"))} -o \"{_outputPath}\" -g \"{_generatedPath}\" {(_namespace != null ? $"-n {_namespace}" : string.Empty)} {(_manifestStartId != null ? $"-id {_manifestStartId}" : string.Empty)}";
            var dialog = new ExternalProcessWindow
            {
                DataContext = new ExternalProcessViewModel(new ExternalProcess(executable, $"{extraArgs} {args}"))
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

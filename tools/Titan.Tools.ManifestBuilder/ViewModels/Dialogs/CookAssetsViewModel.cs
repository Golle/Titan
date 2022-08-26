using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Services;

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
    public CookAssetsViewModel(IDialogService? dialogService = null, IAppSettings? appSettings = null)
    {
        dialogService ??= Registry.GetRequiredService<IDialogService>();
        appSettings ??= Registry.GetRequiredService<IAppSettings>();

        var cookSettings = appSettings.GetSettings().CookAssetSettings;
        _packagePath = cookSettings.OutputPath;
        _generatedPath = cookSettings.GeneratedPath;
        _namespace = cookSettings.Namespace;

        BrowseGeneratedPath = ReactiveCommand.CreateFromTask(async () => GeneratedPath = await dialogService.OpenFolderDialog(_generatedPath));
        BrowsePackagePath = ReactiveCommand.CreateFromTask(async () => PackagePath = await dialogService.OpenFolderDialog(_packagePath));
        Build = ReactiveCommand.CreateFromTask(() =>
        {
            var settings = appSettings.GetSettings();
            appSettings.Save(settings with { CookAssetSettings = new CookAssetSettings(_namespace, PackagePath, GeneratedPath) });
            return Task.CompletedTask;
        });
    }

    public CookAssetsViewModel()
    : this(null)
    {
    }
}



using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class ProjectExplorerViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IManifestService _manifestService;
    private readonly IMessenger _messenger;
    private ManifestViewModel? _selectedManifest;
    private bool _projectLoaded;
    private string? _projectPath;

    public bool ProjectLoaded
    {
        get => _projectLoaded;
        set => SetProperty(ref _projectLoaded, value);
    }
    public ManifestViewModel? SelectedManifest
    {
        get => _selectedManifest;
        set => SetProperty(ref _selectedManifest, value);
    }

    public bool HasManifests => Manifests.Count > 0;
    public IObservableCollection<ManifestViewModel> Manifests { get; } = new ObservableCollectionExtended<ManifestViewModel>();
    public ICommand CreateManifest { get; }

    public ProjectExplorerViewModel(IManifestService? manifestService = null, IDialogService? dialogService = null, IMessenger? messenger = null)
    {
        _dialogService = dialogService ?? Registry.GetRequiredService<IDialogService>();
        _manifestService = manifestService ?? Registry.GetRequiredService<IManifestService>();
        _messenger = messenger ?? Registry.GetRequiredService<IMessenger>();

        CreateManifest = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CreateManifestDialog();
            var name = await dialog.ShowDialog<string?>(App.MainWindow);
            if (name != null)
            {
                var createManifestResult = await _manifestService.CreateManifest(_projectPath!, name);
                if (createManifestResult.Succeeded)
                {
                    var manifest = new ManifestViewModel(createManifestResult.Data!, _messenger);
                    Manifests.Add(manifest);
                    SelectedManifest = manifest;
                    this.RaisePropertyChanged(nameof(HasManifests));
                }
                else
                {
                    await _dialogService.MessageBox("Error", $"Failed to create the manifest with error: {createManifestResult.Error}");
                }
            }
        });
    }

    private async Task LoadManifests()
    {
        var manifests = await _manifestService.ListManifests(_projectPath!);

        if (manifests.Succeeded)
        {
            var viewModels = manifests.Data!.Select(m => new ManifestViewModel(m, _messenger));
            Manifests.Load(viewModels);
        }
        else
        {
            await _dialogService.MessageBox("Error", $"Failed to load the project at path {_projectPath} with message: {manifests.Error}. TBD: how should we handle this?");
            return;
        }
        SelectedManifest = Manifests.FirstOrDefault();
        ProjectLoaded = true;
        this.RaisePropertyChanged(nameof(HasManifests));
    }

    public async Task LoadProject(string path)
    {
        _projectPath = path;
        await LoadManifests();
    }

    public ProjectExplorerViewModel()
        : this(null)
    {
    }
}

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Dialogs;
using Titan.Tools.ManifestBuilder.Views.Dialogs;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ContentViewModel Content { get; }
    public ProjectExplorerViewModel Project { get; }
    public NodePropertiesViewModel Properties { get; }

    public ICommand ExitApplication { get; }
    public ICommand SaveAll { get; }
    public ICommand OpenCookAssetsDialog { get; }
    public MainWindowViewModel(IDialogService? dialogService)
    {
        dialogService ??= Registry.GetRequiredService<IDialogService>();
        Content = new ContentViewModel();
        Project = new ProjectExplorerViewModel();
        Properties = new NodePropertiesViewModel();

        SaveAll = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await Project.SaveAll();
            if (result.Failed)
            {
                await dialogService.MessageBox("Save failed", $"Save failed with error: {result.Error}");
            }
        });

        ExitApplication = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Project.HasUnsavedChanges())
            {
                var result = await dialogService.MessageBox("Unsaved changes", "You've got some unsaved changes. Do you want to save before exiting?", MessageBoxType.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var saveResult = await Project.SaveAll();
                    if (saveResult.Failed)
                    {
                        await dialogService.MessageBox("Save failed", $"Save failed with error: {saveResult.Error}");
                        return;
                    }
                }
            }
            App.Exit();
        });

        OpenCookAssetsDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CookAssetsDialog();
            await dialog.ShowDialog(App.MainWindow);
        });
    }

    public async Task LoadProject(string path)
    {
        await Project.LoadProject(path);

        Content.BasePath = path;
        Content.Load(path);

    }

    public MainWindowViewModel()
    : this(null)
    {

    }
}

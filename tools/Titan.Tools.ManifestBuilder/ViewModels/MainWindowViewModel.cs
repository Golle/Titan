using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ContentViewModel Content { get; }
    public ProjectExplorerViewModel Project { get; }
    public NodePropertiesViewModel Properties { get; }

    public ICommand ExitApplication { get; }
    public MainWindowViewModel()
    {
        Content = new ContentViewModel();
        Project = new ProjectExplorerViewModel();
        Properties = new NodePropertiesViewModel();

        ExitApplication = ReactiveCommand.Create(() => App.Exit());
    }

    public async Task LoadProject(string path)
    {
        await Project.LoadProject(path);

        Content.BasePath = path;
        Content.Load(path);

    }
}

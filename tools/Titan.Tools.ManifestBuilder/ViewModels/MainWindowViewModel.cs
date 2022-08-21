using System.Threading.Tasks;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ContentViewModel Content { get; }
    public ProjectExplorerViewModel Project { get; }
    public FileInfoViewModel Properties { get; }

    public MainWindowViewModel()
    {
        Content = new ContentViewModel();
        Project = new ProjectExplorerViewModel();
    }


    public async Task LoadProject(string path)
    {
        await Project.LoadProject(path);

        Content.BasePath = path;
        Content.Load(path);

    }
}

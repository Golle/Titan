using System;
using System.Threading.Tasks;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ContentViewModel Content { get; }
    public ManifestViewModel Manifest { get; }
    public FileInfoViewModel Properties { get; }

    public MainWindowViewModel()
    {
        Content = new ContentViewModel();
        Manifest = new ManifestViewModel();
    }


    public async Task LoadProject(string path)
    {
        await Manifest.Load(path);
        Content.BasePath = path;
        Content.Load(path);

    }
}

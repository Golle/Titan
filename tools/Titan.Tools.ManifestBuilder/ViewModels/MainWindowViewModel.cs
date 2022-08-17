using System.Threading.Tasks;
using Avalonia.Controls;
using Splat;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase Bottom { get; }
    public ViewModelBase BottomRight { get; }
    public ViewModelBase Left { get; }
    public ViewModelBase Right { get; }

    public MainWindowViewModel()
    {
        if (!Design.IsDesignMode)
        {
            Task.Run(() => Locator.Current.GetRequiredService<IAppConfiguration>()
                    .CreateOrInit(@"f:\git\titan\samples\titan.sandbox\assets"))
                .Wait();
            var fileInfo = new FileInfoViewModel();
            Bottom = new ContentViewModel(fileInfo.FileSelected);
            Left = new ManifestViewModel(Locator.Current.GetRequiredService<IManifestService>());
            Right = new ContentViewModel(fileInfo.FileSelected);
            BottomRight = fileInfo;
        }


    }
}

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase Bottom { get; }
    public ViewModelBase BottomRight { get; }
    public ViewModelBase Left { get; }
    public ViewModelBase Right { get; }

    public MainWindowViewModel()
    {
        var fileInfo = new FileInfoViewModel();
        Bottom = new ContentViewModel(fileInfo.FileSelected);
        Left = new ManifestViewModel();
        Right = new ContentViewModel(fileInfo.FileSelected);

        BottomRight = fileInfo;
    }
}

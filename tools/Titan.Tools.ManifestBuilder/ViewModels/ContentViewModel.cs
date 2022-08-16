using Avalonia.Media;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class ContentViewModel : ViewModelBase
{

    public string ATExt { get; } = "This is a text";
    public SolidColorBrush Background { get; }

    public ContentViewModel()
    {
        Background = new SolidColorBrush(Color.Parse("#00ffff"));
    }
    public ContentViewModel(string background)
    {
        Background = new SolidColorBrush(Color.Parse(background));
    }
}

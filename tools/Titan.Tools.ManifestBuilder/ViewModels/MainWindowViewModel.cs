namespace Titan.Tools.ManifestBuilder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ViewModelBase Bottom { get; }
        public ViewModelBase Left { get; }
        public ViewModelBase Right { get; }

        public MainWindowViewModel()
        {
            Bottom = new ContentViewModel("#0000ff");
            Left = new ContentViewModel("#ff0ff0");
            Right = new ContentViewModel("#ff0f0f");
        }
    }
}

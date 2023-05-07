using Avalonia.Controls;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(App.GetRequiredService<IDialogService>())
        {
            Window = this
        };
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        (DataContext as MainWindowViewModel)?.Startup();
    }
}

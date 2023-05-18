using Avalonia.Controls;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var viewModel = App.GetRequiredService<MainWindowViewModel>();
        viewModel.Window = this;
        DataContext = viewModel;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (!Design.IsDesignMode)
        {
            (DataContext as MainWindowViewModel)?.Startup();
        }
    }
}

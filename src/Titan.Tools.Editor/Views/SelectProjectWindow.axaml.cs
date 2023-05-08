using Avalonia.Controls;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;

public partial class SelectProjectWindow : Window
{
    public SelectProjectWindow()
    {
        InitializeComponent();
        var viewModel = App.GetRequiredService<SelectProjectViewModel>();
        viewModel.Window = this;
        DataContext = viewModel;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (!Design.IsDesignMode)
        {
            (DataContext as SelectProjectViewModel)?.Load();
        }
    }
}

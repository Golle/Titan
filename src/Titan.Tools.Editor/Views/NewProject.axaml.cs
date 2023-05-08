using Avalonia.Controls;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;
public partial class NewProject : Window
{
    public NewProject()
    {
        InitializeComponent();
        var viewModel = App.GetRequiredService<NewProjectViewModel>();
        viewModel.Window = this;
        DataContext = viewModel;
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        (DataContext as NewProjectViewModel)?.LoadTemplates();
    }
}

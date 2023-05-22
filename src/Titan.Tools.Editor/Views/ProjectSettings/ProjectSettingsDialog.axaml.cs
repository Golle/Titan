using Avalonia.Controls;
using Titan.Tools.Editor.ViewModels.ProjectSettings;

namespace Titan.Tools.Editor.Views.ProjectSettings;
public partial class ProjectSettingsDialog : Window
{
    public ProjectSettingsDialog()
    {
        InitializeComponent();

        var viewModel = App.GetRequiredService<ProjectSettingsViewModel>();
        viewModel.Window = this;
        DataContext = viewModel;
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        (DataContext as ProjectSettingsViewModel)?.OnLoaded();
    }
}

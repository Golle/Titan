using Avalonia.Controls;
using Titan.Tools.Editor.ProjectGeneration.Templates;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;
public partial class NewProject : Window
{
    public NewProject()
    {
        InitializeComponent();
        DataContext = new NewProjectViewModel(App.GetRequiredService<IProjectTemplateService>(), App.GetRequiredService<IProjectGenerationService>())
        {
            Window = this
        };
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        (DataContext as NewProjectViewModel)?.LoadTemplates();
    }
}

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.ProjectGeneration.Templates;
using Titan.Tools.Editor.Services;

namespace Titan.Tools.Editor.ViewModels;

internal partial class ProjectTemplateViewModel : ViewModelBase
{
    public required ProjectTemplate Template { get; init; }
    public required Bitmap? Icon { get; init; }

    [ObservableProperty]
    private bool _isSelected;
}

public record NewProjectResult(string Path);
internal partial class NewProjectViewModel : ViewModelBase
{
    private readonly IProjectTemplateService _projectTemplateService;
    private readonly IProjectGenerationService _projectGenerationService;

    [ObservableProperty]
    private ObservableCollection<ProjectTemplateViewModel> _templates = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateProjectCommand))]
    private ProjectTemplateViewModel? _selectedProject;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullProjectLocation))]
    [NotifyCanExecuteChangedFor(nameof(CreateProjectCommand))]
    private string? _projectLocation = GlobalConfiguration.ProjectLocationBaseDir;

    [ObservableProperty]
    private string? _error;

    public string? FullProjectLocation
        => string.IsNullOrWhiteSpace(ProjectLocation) || string.IsNullOrWhiteSpace(Name) ? null : Path.GetFullPath(Path.Combine(ProjectLocation, Name));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullProjectLocation))]
    [NotifyCanExecuteChangedFor(nameof(CreateProjectCommand))]
    private string? _name;
    public NewProjectViewModel(IProjectTemplateService projectTemplateService, IProjectGenerationService projectGenerationService)
    {
        _projectTemplateService = projectTemplateService;
        _projectGenerationService = projectGenerationService;
    }

    [RelayCommand(CanExecute = nameof(CanCreateProject))]
    private async Task CreateProject()
    {
        Error = null;
        var result = await _projectGenerationService.CreateProjectFromTemplate(Name!, ProjectLocation!, SelectedProject!.Template);
        if (result.Success)
        {
            Window?.Close(new NewProjectResult(result.Data!));
        }
        else
        {
            Error = result.Error ?? "Unknown error occured.";
        }
    }

    private bool CanCreateProject()
        => !string.IsNullOrWhiteSpace(ProjectLocation) && !string.IsNullOrWhiteSpace(Name) && SelectedProject != null;

    public async void LoadTemplates()
    {
        Templates.Clear();
        var templates = await _projectTemplateService.GetTemplates();
        foreach (var projectTemplate in templates)
        {
            Templates.Add(new ProjectTemplateViewModel
            {
                Template = projectTemplate,
                Icon = Load(projectTemplate.TemplateImage)
            });
        }
        SelectedProject = Templates.First();
        SelectedProject.IsSelected = true;

        static Bitmap? Load(string? image) => image != null ? new Bitmap(image) : null;
    }

    [RelayCommand]
    private void TemplateSelected(string? templateName)
    {
        //NOTE(Jens): this is some garbage code. Rewrite.
        foreach (var template in Templates)
        {
            template.IsSelected = template.Template.Name == templateName;
            if (template.IsSelected)
            {
                SelectedProject = template;
            }
        }
    }

    public NewProjectViewModel()
        : this(
            App.GetRequiredService<IProjectTemplateService>(),
            App.GetRequiredService<IProjectGenerationService>()
        )
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This constructor should only be used by the designer.");
        }
    }
}


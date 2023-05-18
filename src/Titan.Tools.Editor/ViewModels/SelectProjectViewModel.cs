using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.ViewModels.Dialogs;

namespace Titan.Tools.Editor.ViewModels;
public record SelectProjectResult(string ProjectPath);
internal partial class SelectProjectViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IRecentProjects _recentProjects;

    [ObservableProperty]
    private ObservableCollection<RecentProject> _projects = new();

    public SelectProjectViewModel(IDialogService dialogService, IRecentProjects recentProjects)
    {
        _dialogService = dialogService;
        _recentProjects = recentProjects;
    }

    [RelayCommand]
    private async Task OpenProject()
    {
        var path = await _dialogService.OpenFileDialog(new[] { new FilePickerFileType("Titan Project") { Patterns = new[] { $"*{GlobalConfiguration.TitanProjectFileExtension}" } } });
        if (path != null)
        {
            Window?.Close(new SelectProjectResult(path));
        }
    }

    [RelayCommand]
    private async Task NewProject()
    {
        var result = await _dialogService.OpenNewProjectDialog(Window);
        if (result != null)
        {
            Window?.Close(new SelectProjectResult(result.Path));
        }
    }

    [RelayCommand]
    private void OpenRecentProject(RecentProject? project)
    {
        if (project != null)
        {
            Window?.Close(new SelectProjectResult(project.Path));
        }
    }

    [RelayCommand]
    private async Task RemoveProject(RecentProject? project)
    {
        if (project == null)
        {
            return;
        }
        var result = await _dialogService.ShowMessageBox("Delete Project Reference", $"Are you sure you want to remove the project {project.Name} from the list of recent projects?", MessageBoxType.YesNo);
        if (result is MessageBoxResult.Yes)
        {
            Projects.Remove(project);
            await _recentProjects.Remove(project);
        }
    }

    public SelectProjectViewModel()
        : this(App.GetRequiredService<IDialogService>(), App.GetRequiredService<IRecentProjects>())
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This constructor should only be used in design mode.");
        }
    }

    public async void Load()
    {
        Projects.Clear();
        foreach (var recentProject in await _recentProjects.GetProjects())
        {
            Projects.Add(recentProject);
        }
    }
}

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Services;

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

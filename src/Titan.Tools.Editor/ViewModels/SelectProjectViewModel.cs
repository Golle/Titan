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
    private readonly IAppConfiguration _appConfiguration;

    public SelectProjectViewModel(IDialogService dialogService, IAppConfiguration appConfiguration)
    {
        _dialogService = dialogService;
        _appConfiguration = appConfiguration;
    }

    [ObservableProperty] 
    private ObservableCollection<string> _recentProjects = new();

    [RelayCommand]
    private async Task OpenProject()
    {
        var path = await _dialogService.OpenFileDialog(new[] { new FilePickerFileType("Titan Project") { Patterns = new[] { $"*{GlobalConfiguration.TitanProjectFileExtension}" } } });
        if (path != null)
        {
            await AddToRecentProjects(path);
            Window?.Close(new SelectProjectResult(path));
        }
    }

    [RelayCommand]
    private async Task NewProject()
    {
        var result = await _dialogService.OpenNewProjectDialog(Window);
        if (result != null)
        {
            await AddToRecentProjects(result.Path);
            Window?.Close(new SelectProjectResult(result.Path));
        }
    }

    private async Task AddToRecentProjects(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
        var config = await _appConfiguration.GetConfig();
        if (config.RecentProjects.Add(path))
        {
            await _appConfiguration.SaveConfig(config);
        }
    }

    public SelectProjectViewModel()
    : this(App.GetRequiredService<IDialogService>(), App.GetRequiredService<IAppConfiguration>())
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This constructor should only be used in design mode.");
        }
    }

    public async void Load()
    {
        RecentProjects.Clear();
        var config = await _appConfiguration.GetConfig();
        foreach (var recentProject in config.RecentProjects)
        {
            RecentProjects.Add(recentProject);
        }
    }
}

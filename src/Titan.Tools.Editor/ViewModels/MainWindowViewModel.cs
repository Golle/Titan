using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Services.State;

namespace Titan.Tools.Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ITitanProjectFile _titanProjectFile;
    private readonly IRecentProjects _recentProjects;
    private readonly IApplicationState _applicationState;

    [ObservableProperty]
    private string _greetings = "Welcome to the Titan Editor!";

    [ObservableProperty]
    private ProjectExplorerViewModel? _projectExplorer;
    public ToolbarViewModel Toolbar { get; }
    public TerminalViewModel Terminal { get; }
    public AssetBrowserViewModel Browser { get; }

    
    [RelayCommand]
    private async Task OpenProjectSettings() => await _dialogService.OpenProjectSettingsDialog(Window);
    public MainWindowViewModel(ToolbarViewModel toolbar, TerminalViewModel terminal, AssetBrowserViewModel assetBrowser, IDialogService dialogService, ITitanProjectFile titanProjectFile, IRecentProjects recentProjects, IApplicationState applicationState)
    {
        Toolbar = toolbar;
        Terminal = terminal;
        Browser = assetBrowser;
        _dialogService = dialogService;
        _titanProjectFile = titanProjectFile;
        _recentProjects = recentProjects;
        _applicationState = applicationState;
    }

    public async void Startup()
    {
        var result = await _dialogService.OpenSelectProjectDialog(Window);
        if (result == null)
        {
            App.Exit();
            return;
        }

        var project = await _titanProjectFile.Read(result.ProjectPath);
        if (!project.Success || project.Data == null)
        {
            await _dialogService.ShowMessageBox("Open project failed", project.Error ?? "Unknown error", parent: Window);
            //NOTE(Jens): currenlty no recovery from this. 
            App.Exit(-1);
        }
        await _recentProjects.AddOrUpdateProject(project.Data!.Name, result.ProjectPath);

        //NOTE(Jens): Set the current project
        _applicationState.Initialize(project.Data, result.ProjectPath);

        //NOTE(Jens): implement the rest of the view models. should we maybe use service collection here?
        ProjectExplorer = new ProjectExplorerViewModel();
        Toolbar.IsProjectLoaded = true;
        Greetings = $"Path: {result.ProjectPath}";
        await Browser.LoadContents();
    }

    public async void Shutdown()
    {
        await _applicationState.Stop();
    }

    #region DESIGNER

    public MainWindowViewModel()
        : this(
            App.GetRequiredService<ToolbarViewModel>(),
            App.GetRequiredService<TerminalViewModel>(),
            App.GetRequiredService<AssetBrowserViewModel>(),
            App.GetRequiredService<IDialogService>(),
            App.GetRequiredService<ITitanProjectFile>(),
            App.GetRequiredService<IRecentProjects>(),
            App.GetRequiredService<IApplicationState>()
        )
    {
        Helper.CheckDesignMode(nameof(MainWindowViewModel));
    }

    #endregion
}

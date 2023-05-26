using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Services.State;

namespace Titan.Tools.Editor.ViewModels;

public interface ITool
{
    string Name { get; }
    Task<Result> Execute();
}

public partial class ToolbarViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IApplicationState _applicationState;

    [ObservableProperty]
    private ObservableCollection<string> _configurations = new();

    [ObservableProperty]
    private bool _isProjectLoaded;
    public ITool[] Tools { get; }

    private string? _selecteed;
    public string? SelectedConfiguration
    {
        set
        {
            _selecteed = value;
            ChangeConfigurationCommand.Execute(_selecteed);
            OnPropertyChanged();
        }
        get => _selecteed;
    }

    [RelayCommand]
    private async Task Execute(ITool tool)
    {
        var result = await tool.Execute();
        if (!result.Success)
        {
            await _dialogService.ShowMessageBox($"{tool.Name} failed", $"The command failed. Error = {result.Error}");
        }
    }

    [RelayCommand]
    private async Task ChangeConfiguration(string? config)
    {
        if (config == null)
        {
            return;
        }

        var buildSettings = _applicationState.Project.BuildSettings;
        if (buildSettings.CurrentConfiguration == config)
        {
            return;
        }
        if (buildSettings.Configurations.All(c => c.Name != config))
        {
            throw new InvalidOperationException($"The config '{config}' does not exist.");
        }

        buildSettings.CurrentConfiguration = config;
        await _applicationState.SaveChanges();
    }

    public Task LoadContents()
    {
        Configurations.Clear();
        var buildSettings = _applicationState.Project.BuildSettings;
        var configurations = buildSettings
            .Configurations
            .Select(c => c.Name);
        Configurations.AddRange(configurations);
        SelectedConfiguration = buildSettings.CurrentConfiguration;
        IsProjectLoaded = true;
        return Task.CompletedTask;
    }

    public ToolbarViewModel(IEnumerable<ITool> tools, IDialogService dialogService, IApplicationState applicationState)
    {
        _dialogService = dialogService;
        _applicationState = applicationState;
        Tools = tools.ToArray();
    }

    public ToolbarViewModel()
        : this(
            App.GetRequiredService<IEnumerable<ITool>>(),
            App.GetRequiredService<IDialogService>(),
            App.GetRequiredService<IApplicationState>()
        )
    {
        Helper.CheckDesignMode(nameof(ToolbarViewModel));
    }
}


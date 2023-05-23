using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Services.State;

namespace Titan.Tools.Editor.ViewModels.ProjectSettings;

internal partial class GameConfigurationViewModel : ViewModelBase
{

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _newName;

    [ObservableProperty]
    private string? _configuration;

    [ObservableProperty]
    private bool _nativeAOT;

    [ObservableProperty]
    private bool _trimming;

    public void SaveChanges(GameBuildConfiguration config)
    {
        config.NativeAOT = NativeAOT;
        config.Configuration = Configuration ?? string.Empty;
        config.Trimming = Trimming;
        Name = config.Name = NewName ?? string.Empty;
    }
}

internal partial class BuildSettingsViewModel : ViewModelBase, IProjectSettings
{
    private readonly IApplicationState _applicationState;
    private readonly IDialogService _dialogService;
    public string Name => "Build";

    [ObservableProperty]
    private GameConfigurationViewModel? _selectedConfiguration;

    [ObservableProperty]
    private ObservableCollection<GameConfigurationViewModel> _configurations = new();
    public BuildSettingsViewModel(IApplicationState applicationState, IDialogService dialogService)
    {
        _applicationState = applicationState;
        _dialogService = dialogService;

        var configs = _applicationState.Project.BuildSettings
            .Configurations.Select(c => new GameConfigurationViewModel
            {
                Name = c.Name,
                NewName = c.Name,
                NativeAOT = c.NativeAOT,
                Trimming = c.Trimming,
                Configuration = c.Configuration,
            });
        _configurations.AddRange(configs);
        _selectedConfiguration = _configurations.FirstOrDefault();
    }

    [RelayCommand]
    private void AddNewConfiguration()
    {
        var config = new GameConfigurationViewModel
        {
            Name = "NewConfig",
            NewName = "NewConfig",
            Configuration = "NewConfig",
        };
        Configurations.Add(config);
        SelectedConfiguration = config;
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        var buildSettings = _applicationState.Project.BuildSettings;
        foreach (var config in Configurations)
        {
            var buildConfig = buildSettings.Configurations.FirstOrDefault(c => c.Name == config.Name);
            if (buildConfig == null)
            {
                buildConfig = new GameBuildConfiguration
                {
                    Name = config.Name ?? string.Empty,
                    Configuration = config.Configuration ?? string.Empty
                };
                buildSettings.Configurations.Add(buildConfig);
            }
            config.SaveChanges(buildConfig);
        }
        var result = await _applicationState.SaveChanges();
        if (!result.Success)
        {
            await _dialogService.ShowMessageBox("ERROR", result.Error);
        }
    }


    public BuildSettingsViewModel()
        : this(
            App.GetRequiredService<IApplicationState>(),
            App.GetRequiredService<IDialogService>()
        )
    {
        Helper.CheckDesignMode(nameof(BuildSettingsViewModel));
    }
}

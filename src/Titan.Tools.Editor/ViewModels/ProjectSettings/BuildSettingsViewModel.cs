using System.Collections.ObjectModel;
using Avalonia.Media.Fonts;
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

    [ObservableProperty]
    private bool _deleted;

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
    private string? _outputPath;

    [ObservableProperty]
    private ObservableCollection<GameConfigurationViewModel> _configurations = new();
    public BuildSettingsViewModel(IApplicationState applicationState, IDialogService dialogService)
    {
        _applicationState = applicationState;
        _dialogService = dialogService;
        LoadConfigurations();
    }

    private void LoadConfigurations()
    {
        Configurations.Clear();
        var buildSettings = _applicationState.Project.BuildSettings;
        var configs = buildSettings
            .Configurations.Select(c => new GameConfigurationViewModel
            {
                Name = c.Name,
                NewName = c.Name,
                NativeAOT = c.NativeAOT,
                Trimming = c.Trimming,
                Configuration = c.Configuration,
            });
        Configurations.AddRange(configs);
        SelectedConfiguration = Configurations.FirstOrDefault();
        OutputPath = buildSettings.OutputPath;
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
        if (Configurations.All(c => c.Deleted))
        {
            await _dialogService.ShowMessageBox("ERROR", "There must be atleast one configuration.");
            return;
        }

        if (Configurations.Select(c => c.NewName).Distinct().Count() != Configurations.Count)
        {
            await _dialogService.ShowMessageBox("ERROR", "Multiple configurations with the same name.");
            return;
        }

        var buildSettings = _applicationState.Project.BuildSettings;
        buildSettings.OutputPath = string.IsNullOrEmpty(OutputPath)
            ? TitanProjectBuildSettings.DefaultOutputPath
            : GetRelativePath(_applicationState.ProjectDirectory, OutputPath);

        foreach (var config in Configurations)
        {
            if (config.Deleted)
            {
                buildSettings.Configurations.RemoveAll(c => c.Name == config.Name);
                continue;
            }

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

        // reload all the configs if we deleted one.
        if (Configurations.Any(c => c.Deleted))
        {
            LoadConfigurations();
        }
    }

    private static string GetRelativePath(string basePath, string path)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetRelativePath(basePath, path);
        }
        return path;
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

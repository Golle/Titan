using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Services.State;

namespace Titan.Tools.Editor.ViewModels.ProjectSettings;

internal partial class BuildSettingsViewModel : ViewModelBase, IProjectSettings
{
    private readonly IApplicationState _applicationState;
    private readonly IDialogService _dialogService;
    public string Name => "Build";

    [ObservableProperty] 
    private bool _nativeAOT;

    [ObservableProperty]
    private bool _trimming;

    public BuildSettingsViewModel(IApplicationState applicationState, IDialogService dialogService)
    {
        _applicationState = applicationState;
        _dialogService = dialogService;

        var buildSettings = _applicationState.Project.BuildSettings;
        _nativeAOT = buildSettings.NativeAOT;
        _trimming = buildSettings.Trimming;
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        _applicationState.Project.BuildSettings.NativeAOT = NativeAOT;
        _applicationState.Project.BuildSettings.Trimming = Trimming;
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

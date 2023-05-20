using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Titan.Platform.Win32;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Tools;

namespace Titan.Tools.Editor.ViewModels;

public interface ITool
{
    string Name { get; }
    Task<Result> Execute();
}

public partial class ToolbarViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private bool _isProjectLoaded;

    public ITool[] Tools { get; }

    [RelayCommand]
    private async Task Execute(ITool tool)
    {
        var result = await tool.Execute();
        if (!result.Success)
        {
            await _dialogService.ShowMessageBox($"{tool.Name} failed", $"The command failed. Error = {result.Error}");
        }
    }
    public ToolbarViewModel(ToolsProvider toolsProvider, IDialogService dialogService)
    {
        _dialogService = dialogService;
        Tools = toolsProvider
            .GetTools()
            .ToArray();
    }

    public ToolbarViewModel()
        : this(
            App.GetRequiredService<ToolsProvider>(),
            App.GetRequiredService<IDialogService>()
        )
    {
        Helper.CheckDesignMode(nameof(ToolbarViewModel));
    }
}


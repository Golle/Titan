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

    public SelectProjectViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    [ObservableProperty]
    private ObservableCollection<string> _recentProjects = new() { "This is an item!", "And another one" };

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
    : this(App.GetRequiredService<IDialogService>())
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This constructor should only be used in design mode.");
        }
    }
}

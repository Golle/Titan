using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Titan.Tools.Editor.Services;

namespace Titan.Tools.Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _greetings = "Welcome to the Titan Editor!";

    [ObservableProperty]
    private ProjectExplorerViewModel? _projectExplorer;

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async void Startup()
    {
        var result = await _dialogService.OpenSelectProjectDialog(Window);
        if (result == null)
        {
            App.Exit();
            return;
        }

        //NOTE(Jens): implement the rest of the view models. should we maybe use service collection here?
        ProjectExplorer = new ProjectExplorerViewModel();
        Greetings = $"Path: {result.ProjectPath}";
    }

    public MainWindowViewModel()
    : this(App.GetRequiredService<IDialogService>())
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This constructor should only be used by the designer.");
        }
    }
}

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Titan.Tools.Editor.Services;

namespace Titan.Tools.Editor.ViewModels;

internal partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _greetings = "Welcome to the Titan Editor!";

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
        }

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

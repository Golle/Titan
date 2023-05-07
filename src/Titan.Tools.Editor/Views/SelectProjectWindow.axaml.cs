
using Avalonia.Controls;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;

public partial class SelectProjectWindow : Window
{
    public SelectProjectWindow()
    {
        InitializeComponent();
        DataContext = new SelectProjectViewModel(App.GetRequiredService<IDialogService>(), App.GetRequiredService<IAppConfiguration>())
        {
            Window = this
        };
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (Design.IsDesignMode)
        {
            return;
        }

        (DataContext as SelectProjectViewModel)?.Load();
    }
}

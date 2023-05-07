
using Avalonia.Controls;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Views;

public partial class SelectProjectWindow : Window
{
    public SelectProjectWindow()
    {
        InitializeComponent();
        DataContext = new SelectProjectViewModel(App.GetRequiredService<IDialogService>())
        {
            Window = this
        };
    }
}

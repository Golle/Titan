using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Titan.Tools.Editor.ViewModels;
public partial class ViewModelBase : ObservableObject
{
    public Window? Window { get; set; }
}

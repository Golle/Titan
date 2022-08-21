using System.Linq;
using Avalonia.Controls;
using DynamicData.Binding;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifest;

public class ManifestViewModel : ViewModelBase
{
    private ManifestTreeNodeViewModel? _selectedNode;
    public required string Name { get; init; }
    public ManifestTreeNodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set => SetProperty(ref _selectedNode, value);
    }
    public required IObservableCollection<ManifestTreeNodeViewModel> Nodes { get; init; }
    public static ManifestViewModel CreateFromManifest(Models.Manifest manifest)
    {
        var nodes = new[]
        {
            new ManifestTreeNodeViewModel("Textures", manifest.Textures.Select(m => new ManifestTreeNodeViewModel(System.IO.Path.GetFileNameWithoutExtension(m.Path)))),
            new ManifestTreeNodeViewModel("Models", manifest.Models.Select(m => new ManifestTreeNodeViewModel(System.IO.Path.GetFileNameWithoutExtension(m.Path))))
        };

        return new ManifestViewModel
        {
            Name = manifest.Name,
            Nodes = new ObservableCollectionExtended<ManifestTreeNodeViewModel>(nodes)
        };
    }

    public ManifestViewModel()
    {
        if (Design.IsDesignMode)
        {
            Name = "Deisgn mode manifest";
            Nodes = new ObservableCollectionExtended<ManifestTreeNodeViewModel>(new[]
            {
                new ManifestTreeNodeViewModel("Test_01", new []{new ManifestTreeNodeViewModel("Child 01"), new ManifestTreeNodeViewModel("Child 02")}),
                new ManifestTreeNodeViewModel("Test_02", new []{new ManifestTreeNodeViewModel("Child 01"), new ManifestTreeNodeViewModel("Child 02", new[] {new ManifestTreeNodeViewModel("Sub child 01")})})
            });
        }
    }
}

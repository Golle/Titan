using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class ManifestViewModel2 : ViewModelBase
{
    private ManifestTreeNodeViewModel? _selectedNode;
    public required string Name { get; init; }
    public ManifestTreeNodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set => SetProperty(ref _selectedNode, value);
    }
    public required IObservableCollection<ManifestTreeNodeViewModel> Nodes { get; init; }
    public static ManifestViewModel2 CreateFromManifest(Manifest manifest)
    {
        var nodes = new[]
        {
            new ManifestTreeNodeViewModel("Textures", manifest.Textures.Select(m => new ManifestTreeNodeViewModel(System.IO.Path.GetFileNameWithoutExtension(m.Path)))),
            new ManifestTreeNodeViewModel("Models", manifest.Models.Select(m => new ManifestTreeNodeViewModel(System.IO.Path.GetFileNameWithoutExtension(m.Path))))
        };

        return new ManifestViewModel2
        {
            Name = manifest.Name,
            Nodes = new ObservableCollectionExtended<ManifestTreeNodeViewModel>(nodes)
        };
    }

    public ManifestViewModel2()
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

public class ManifestTreeNodeViewModel : ViewModelBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    public IObservableCollection<ManifestTreeNodeViewModel> Children { get; } = new ObservableCollectionExtended<ManifestTreeNodeViewModel>();

    public ManifestTreeNodeViewModel(string name)
        : this(name, Enumerable.Empty<ManifestTreeNodeViewModel>())
    {

    }
    public ManifestTreeNodeViewModel(string name, IEnumerable<ManifestTreeNodeViewModel> children)
    {
        _name = name;
        Children.AddRange(children);
    }
}

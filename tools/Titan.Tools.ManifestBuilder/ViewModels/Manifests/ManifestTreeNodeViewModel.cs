using System.Collections.Generic;
using System.Linq;
using DynamicData;
using DynamicData.Binding;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public class ManifestTreeNodeViewModel<TItem> : ViewModelBase, IManifestTreeNode where TItem : ManifestItem
{
    public TItem Item => _item;
    protected TItem _item;

    [EditorString(Watermark = "Node name")]
    [Order(int.MaxValue)] // put Name at the top
    public string Name
    {
        get => _item.Name;
        set => SetProperty(() => _item.Name = value);
    }

    public IObservableCollection<IManifestTreeNode> Children { get; } = new ObservableCollectionExtended<IManifestTreeNode>();

    public ManifestTreeNodeViewModel(TItem item)
        : this(item, Enumerable.Empty<IManifestTreeNode>())
    {
    }

    public ManifestTreeNodeViewModel(TItem item, IEnumerable<IManifestTreeNode> children)
    {
        _item = item;
        Children.AddRange(children);
    }
}

public class ManifestTreeNodeViewModel : ViewModelBase, IManifestTreeNode
{
    [EditorReadOnly]
    [Order(int.MaxValue)]
    public string Name { get; set; }
    public IObservableCollection<IManifestTreeNode> Children { get; } = new ObservableCollectionExtended<IManifestTreeNode>();

    public ManifestTreeNodeViewModel()
    {
        Name = string.Empty;
    }
    public ManifestTreeNodeViewModel(string name)
        : this(name, Enumerable.Empty<IManifestTreeNode>())
    {
    }
    public ManifestTreeNodeViewModel(string name, IEnumerable<IManifestTreeNode> children)
    {
        Name = name;
        Children.AddRange(children);
    }
}

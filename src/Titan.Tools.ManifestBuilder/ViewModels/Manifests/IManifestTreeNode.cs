using DynamicData.Binding;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public interface IManifestTreeNode
{
    string Name { get; }
    IObservableCollection<IManifestTreeNode> Children { get; }
}

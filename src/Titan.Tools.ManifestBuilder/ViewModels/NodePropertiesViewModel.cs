using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class NodePropertiesViewModel : ViewModelBase
{
    private IManifestTreeNode? _node;
    public IManifestTreeNode? Node
    {
        get => _node;
        set => SetProperty(ref _node, value);
    }
    public NodePropertiesViewModel(IMessenger? messenger)
    {
        messenger ??= Registry.GetRequiredService<IMessenger>();
        messenger.Subscribe<ManifestNodeSelected>(this, message => Node = message.Node);
    }

    public NodePropertiesViewModel()
    : this(null)
    {
    }
}

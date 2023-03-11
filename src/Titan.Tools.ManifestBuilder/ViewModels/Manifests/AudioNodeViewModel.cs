using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public class AudioNodeViewModel : ManifestTreeNodeViewModel<AudioItem>
{
    [EditorReadOnly]
    [Order(int.MaxValue - 1)] // put path at the top. but below name
    public string Path => _item.Path;
    public AudioNodeViewModel(AudioItem item) 
        : base(item)
    {
    }
}

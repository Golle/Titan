using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public class ModelNodeViewModel : ManifestTreeNodeViewModel<ModelItem>
{
    public string Path => _item.Path;
    public ModelNodeViewModel(ModelItem model)
        : base(model)
    {
    }
}

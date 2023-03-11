using Titan.Tools.Core;
using Titan.Tools.Core.Manifests;
using Titan.Tools.Core.Shaders;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public class ShaderNodeViewModel : ManifestTreeNodeViewModel<ShaderItem>
{

    [EditorString]
    public string EntryPoint { get => _item.EntryPoint; set => SetProperty(() => _item.EntryPoint = value); }
    [EditorEnum]
    public ShaderModels ShaderModel { get => _item.ShaderModel; set => SetProperty(() => _item.ShaderModel = value); }

    public ShaderNodeViewModel(ShaderItem item) 
        : base(item)
    {
    }
}

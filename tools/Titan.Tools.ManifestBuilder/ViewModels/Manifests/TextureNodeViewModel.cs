using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public class TextureNodeViewModel : ManifestTreeNodeViewModel<TextureItem>
{
    [EditorReadOnly]
    [Order(int.MaxValue - 1)] // put path at the top. but below name
    public string Path => _item.Path;

    [EditorEnum]
    public TextureType Type
    {
        get => _item.Type;
        set => SetProperty(() => _item.Type = value);
    }

    public TextureNodeViewModel(TextureItem textureItem)
        : base(textureItem)
    {
    }
}

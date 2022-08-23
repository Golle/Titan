using System.ComponentModel;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;

public class MaterialNodeViewModel : ManifestTreeNodeViewModel<MaterialItem>
{
    [EditorNumber]
    public float Alpha { get => _item.Alpha; set => SetProperty(() => _item.Alpha = value); }
    [EditorNumber]
    public float Transparency { get => _item.Transparency; set => SetProperty(() => _item.Transparency = value); }
    [EditorNumber]
    public float Shininess { get => _item.Shininess; set => SetProperty(() => _item.Shininess = value); }
    [EditorNumber]
    public float Illumination { get => _item.Illumination; set => SetProperty(() => _item.Illumination = value); }

    [EditorString]
    public string AmbientColor { get => _item.AmbientColor; set => SetProperty(() => _item.AmbientColor = value); }
    [EditorString]
    public string DiffuseColor { get => _item.DiffuseColor; set => SetProperty(() => _item.DiffuseColor = value); }
    [EditorString]
    public string SpecularColor { get => _item.SpecularColor; set => SetProperty(() => _item.SpecularColor = value); }
    [EditorString]
    public string EmissiveColor { get => _item.EmissiveColor; set => SetProperty(() => _item.EmissiveColor = value); }

    public MaterialNodeViewModel(MaterialItem item)
        : base(item)
    {
    }
}

using Avalonia.Threading;
using Titan.Tools.Core.Manifests;
using Titan.Tools.Core.Models.WavefrontObj;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;

namespace Titan.Tools.ManifestBuilder.ViewModels.Previews;

public record MeshInfo(int Vertices, int Normals, int TextureCoordinates, int Materials, SubMesh[] SubMeshes);
public record SubMesh(string Name, string Material, int Indices, int Smoothing);

public class ModelPreviewViewModel : PreviewViewModelBase
{
    private readonly IApplicationState _appState;
    public ModelNodeViewModel Model { get; }

    private MeshInfo? _mesh;
    public MeshInfo? Mesh
    {
        get => _mesh;
        set => SetProperty(ref _mesh, value);
    }

    public ModelPreviewViewModel(ModelNodeViewModel model, IApplicationState? appState = null)
    {
        Model = model;
        _appState = appState ?? Registry.GetRequiredService<IApplicationState>();
    }

    public override Task Load()
    {
        var uiThread = Dispatcher.UIThread.CheckAccess();
        var obj = ObjParser.Load(Path.Combine(_appState.ProjectPath!, Model.Path));
        if (obj != null)
        {
            var submeshes = obj.Objects.SelectMany(o => o.Groups.Select(g => new SubMesh($"{o.Name}_{g.Name}".Trim('_'), (g.MaterialIndex != -1 ? obj.Materials[g.MaterialIndex].Name : string.Empty), g.NormalIndices.Count, g.Smoothing)));
            Mesh = new MeshInfo(obj.Vertices.Length, obj.Normals.Length, obj.Textures.Length, obj.Materials.Length, submeshes.ToArray());
        }
        return Task.CompletedTask;
    }

    public ModelPreviewViewModel()
    : this(new ModelNodeViewModel(new ModelItem()))
    {

    }
}


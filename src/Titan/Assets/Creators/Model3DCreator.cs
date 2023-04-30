using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.Resources;

namespace Titan.Assets.Creators;

internal struct Model3DCreator : IResourceCreator<Model3D>
{
    private ObjectHandle<IResourceManager> _resourceManager;
    public static AssetDescriptorType Type => AssetDescriptorType.Model;
    public bool Init(in ResourceCreatorInitializer initializer)
    {
        _resourceManager = initializer.GetManagedResource<IResourceManager>();
        return true;
    }

    public Handle<Model3D> Create(in AssetDescriptor descriptor, TitanBuffer data)
    {
        Debug.Assert(descriptor.Type is AssetDescriptorType.Model);

        ref readonly var desc = ref descriptor.Model;

        var handle = _resourceManager.Value.CreateModel(new CreateModel3DArgs(desc.Vertices, desc.Normals, desc.UVs, desc.IndexCount, desc.IndexSize, data));
        if (handle.IsInvalid)
        {
            Logger.Error<Model3DCreator>($"Failed to create the Model3D. ID: {descriptor.Id} Manifest: {descriptor.ManifestId}");
        }
        return handle;
    }

    public bool Recreate(in Handle<Model3D> handle, in AssetDescriptor descriptor, TitanBuffer data)
    {
        Logger.Warning<Model3DCreator>("ReCreate has not been implemented, and might never be.");
        return false;
    }

    public void Destroy(Handle<Model3D> handle)
    {
        _resourceManager.Value.DestroyModel(handle);
    }

    public void Release()
    {
        Logger.Warning<Model3DCreator>("Release has not been implemented.");
    }
}

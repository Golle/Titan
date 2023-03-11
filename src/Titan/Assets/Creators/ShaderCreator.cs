using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.Resources;

namespace Titan.Assets.Creators;

internal struct ShaderCreator : IResourceCreator<Shader>
{
    private ObjectHandle<IResourceManager> _resourceManager;
    public static AssetDescriptorType Type => AssetDescriptorType.Shader;
    public bool Init(in ResourceCreatorInitializer initializer)
    {
        _resourceManager = initializer.GetManagedResource<IResourceManager>();
        return true;
    }

    public Handle<Shader> Create(in AssetDescriptor descriptor, TitanBuffer data)
    {
        Debug.Assert(descriptor.Type == AssetDescriptorType.Shader);
        var manager = _resourceManager.Value;

        var handle = manager.CreateShader(new CreateShaderArgs(descriptor.Shader.ShaderType, data));
        if (handle.IsInvalid)
        {
            Logger.Error<ShaderCreator>("Failed to create the Shader");
        }
        return handle;
    }

    public bool Recreate(in Handle<Shader> handle, in AssetDescriptor descriptor, TitanBuffer data)
    {
        throw new NotImplementedException();
    }

    public void Destroy(Handle<Shader> handle)
    {
        var manager = _resourceManager.Value;
        manager.DestroyShader(handle);
    }

    public void Release()
    {
        Logger.Warning<ShaderCreator>("Release has not been implemented.");
    }
}

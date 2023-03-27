using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.Resources;

namespace Titan.Assets.Creators;

internal struct TextureCreator : IResourceCreator<Texture>
{
    public static AssetDescriptorType Type => AssetDescriptorType.Texture;
    private ObjectHandle<IResourceManager> _resourceManager;

    public bool Init(in ResourceCreatorInitializer initializer)
    {
        _resourceManager = initializer.GetManagedResource<IResourceManager>();
        return true;
    }

    public void Release()
    {
        Logger.Info<TextureCreator>("Release");
    }

    public Handle<Texture> Create(in AssetDescriptor descriptor, TitanBuffer data)
    {
        Debug.Assert(descriptor.Type == Type);
        var manager = _resourceManager.Value;
        ref readonly var desc = ref descriptor.Image;
        Logger.Info<TextureCreator>($"Creating texture with Size {desc.Width} x {desc.Height} and format {(TextureFormat)desc.Format}");
        var args = new CreateTextureArgs(desc.Width, desc.Height, (TextureFormat)desc.Format, InitialData: data);
        var handle = manager.CreateTexture(args);
        if (handle.IsInvalid)
        {
            Logger.Error<TextureCreator>($"Failed to create the texture. ID: {descriptor.Id} Manifest: {descriptor.ManifestId}");
        }
        return handle;
    }

    public bool Recreate(in Handle<Texture> handle, in AssetDescriptor descriptor, TitanBuffer data)
    {
        var manager = _resourceManager.Value;
        if (!manager.UploadTexture(handle, data))
        {
            
            Logger.Error<TextureCreator>("Failed to upload the texture to the GPU");
        }
        return true;
    }

    public void Destroy(Handle<Texture> handle)
    {
        Debug.Assert(handle.IsValid);
        Logger.Trace<TextureCreator>($"Destroying texture with handle: {handle}");
        _resourceManager.Value.Destroy(handle);
    }
}

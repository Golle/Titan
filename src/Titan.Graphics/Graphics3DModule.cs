using System.Diagnostics;
using System;
using System.Threading;
using Titan.Assets.NewAssets;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.Graphics.D3D12Take2;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics;

public struct Graphics3DModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        builder
            .AddModule<D3D12RenderModule1>()
            .AddModule<GraphicsLoaderModule>();

        return true;
    }
}

internal unsafe struct GraphicsLoaderModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        if (!ValidateDependencies(builder))
        {
            Logger.Error<GraphicsLoaderModule>($"Failed to validate the dependencies for {nameof(GraphicsLoaderModule)}");
            return false;
        }

        // register all creator resources
        builder
            .AddResource<TextureCreator>();

        var registry = builder.GetResourcePointer<ResourceCreatorRegistry>();
        var textureCreator = builder.GetResourcePointer<TextureCreator>();

        if (!textureCreator->Initialize(builder.GetResource<D3D12Core>().Device))
        {
            Logger.Error<GraphicsLoaderModule>($"Failed to initialize {nameof(TextureCreator)}");
            return false;
        }

        if (!registry->Register<TexturePLACEHOLDER, TextureCreator>(AssetDescriptorType.Texture, textureCreator))
        {
            Logger.Error<GraphicsLoaderModule>($"Failed to register asset type {AssetDescriptorType.Texture} for {nameof(TexturePLACEHOLDER)} with creator {nameof(TextureCreator)}");
            return false;
        }

        return true;
    }

    private static bool ValidateDependencies(AppBuilder builder)
    {
        return
            CheckResource<D3D12Core>() &&
            CheckResource<ResourceCreatorRegistry>();

        bool CheckResource<T>() where T : unmanaged
        {
            if (builder.HasResource<T>())
            {
                return true;
            }
            Logger.Error<GraphicsLoaderModule>($"Resource {typeof(T).Name} has not been registed.");
            return false;

        }
    }
}
internal struct TexturePLACEHOLDER { }
internal unsafe struct TextureCreator : IResource, IResourceCreator<TexturePLACEHOLDER>
{
    //NOTE(Jens): this is just a sample creator.
    private ComPtr<ID3D12Device4> _device;
    private volatile int _value;

    public bool Initialize(ID3D12Device4* device)
    {
        Debug.Assert(_device.Get() == null);
        _device = device;
        return true;
    }

    public void Release()
    {
        _device.Reset();
    }
    public static Handle<TexturePLACEHOLDER> Create(void* context, ReadOnlySpan<byte> data)
    {
        var ptr = (TextureCreator*)context;
        // do the stuffs
        var handle = Interlocked.Increment(ref ptr->_value);
        Logger.Info<TextureCreator>($"Created {handle}");
        return handle;
    }

    public static void Destroy(void* context, Handle<TexturePLACEHOLDER> handle)
    {
        Logger.Info<TextureCreator>($"Destroyed {handle}");
        var ptr = (TextureCreator*)context;
        // do the stuffs
        Interlocked.Decrement(ref ptr->_value);
    }
}

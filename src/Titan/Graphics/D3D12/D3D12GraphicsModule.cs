using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.D3D12.Upload;
using Titan.Graphics.Rendering;
using Titan.Graphics.Resources;
using Titan.Modules;
using Titan.Platform.Win32.D3D;
using Titan.Setup;
using Titan.Windows;
using static Titan.Platform.Win32.D3D.D3D_FEATURE_LEVEL;

namespace Titan.Graphics.D3D12;

internal struct D3D12GraphicsModule : IModule
{
    private const D3D_FEATURE_LEVEL MinFeatureLevel = D3D_FEATURE_LEVEL_11_1;
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource<IGraphicsAdapter>(new DXGIAdapter())
            .AddManagedResource<IGraphicsDevice>(new D3D12GraphicsDevice())
            .AddManagedResource<ISwapChain>(new DXGISwapChain())
            .AddManagedResource<IResourceManager>(new D3D12ResourceManager())
            .AddManagedResource(new D3D12CommandQueue())
            .AddManagedResource(new D3D12UploadQueue())
            .AddManagedResource(new D3D12Allocator())
            ;
        return true;
    }

    public static bool Init(IApp app)
    {
        var adapter = app.GetManagedResource<IGraphicsAdapter>() as DXGIAdapter;
        var device = app.GetManagedResource<IGraphicsDevice>() as D3D12GraphicsDevice;
        var swapchain = app.GetManagedResource<ISwapChain>() as DXGISwapChain;
        var resourceManager = app.GetManagedResource<IResourceManager>() as D3D12ResourceManager;
        var uploadQueue = app.GetManagedResource<D3D12UploadQueue>();
        var commandQueue = app.GetManagedResource<D3D12CommandQueue>();
        var allocator = app.GetManagedResource<D3D12Allocator>();

        Debug.Assert(adapter != null && device != null && swapchain != null && resourceManager != null);

        var window = app.GetManagedResource<IWindow>();
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        var config = app.GetConfigOrDefault<GraphicsConfig>();

        if (!adapter.Init(config.Debug, MinFeatureLevel))
        {
            Logger.Error<D3D12GraphicsModule>("Failed to find a compatible adapter.");
            return false;
        }
        if (!device.Init(adapter, MinFeatureLevel, config.Debug))
        {
            Logger.Error<D3D12GraphicsModule>($"Failed to init the {nameof(D3D12GraphicsDevice)}.");
            return false;
        }

        if (!commandQueue.Init(device, memoryManager, resourceManager, 10, config.TripleBuffering ? 3u : 2u))
        {
            Logger.Error<D3D12GraphicsModule>($"Failed to init the {nameof(D3D12CommandQueue)}");
            return false;
        }

        if (!allocator.Init(device, memoryManager, config.TripleBuffering ? 3u : 2u, config.MemoryConfig))
        {
            Logger.Error<D3D12GraphicsModule>($"Failed to init the {nameof(D3D12Allocator)}.");
            return false;
        }

        if (!swapchain.Init(device, allocator, commandQueue, window, config))
        {
            Logger.Error<D3D12GraphicsModule>($"Failed to init the {nameof(DXGISwapChain)}.");
            return false;
        }

        if (!uploadQueue.Init(memoryManager, device, config.ResourcesConfig.UploadFrames))
        {
            Logger.Error<D3D12GraphicsModule>($"Failed to init the {nameof(D3D12UploadQueue)}.");
            return false;
        }

        if (!resourceManager.Init(config.ResourcesConfig, memoryManager, device, uploadQueue, allocator))
        {
            Logger.Error<D3D12GraphicsModule>($"Failed to init the {nameof(D3D12ResourceManager)}.");
            return false;
        }

        return true;
    }


    public static bool Shutdown(IApp app)
    {
        var adapter = (DXGIAdapter)app.GetManagedResource<IGraphicsAdapter>();
        var device = (D3D12GraphicsDevice)app.GetManagedResource<IGraphicsDevice>();
        var swapChain = (DXGISwapChain)app.GetManagedResource<ISwapChain>();
        var resourceManager = (D3D12ResourceManager)app.GetManagedResource<IResourceManager>();
        var uploadQueue = app.GetManagedResource<D3D12UploadQueue>();
        var commandQueue = app.GetManagedResource<D3D12CommandQueue>();
        var allocator = app.GetManagedResource<D3D12Allocator>();
        uploadQueue.Shutdown();
        swapChain.Shutdown();
        commandQueue.Shutdown();
        adapter.Shutdown();
        device.Shutdown();
        resourceManager.Shutdown();
        allocator.Shutdown();
        return true;
    }
}

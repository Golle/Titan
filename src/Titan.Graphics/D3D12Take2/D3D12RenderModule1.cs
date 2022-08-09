using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.D3D12Take2.Stats;
using Titan.Graphics.D3D12Take2.Systems;
using Titan.Graphics.Modules;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;

namespace Titan.Graphics.D3D12Take2;

public unsafe struct D3D12RenderModule1 : IModule
{
    private const uint BufferCount = 3;

    public static void Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();

        // add some config that we can check if debug should be enabled
        EnableDebugLayer();

        // factory and adapter are only used to create device and swapchain, can be disposed at the end of the function.
        using DXGIFactory factory = default;
        DXGIAdapter adapter = default;

        D3D12Device device = default;
        D3D12Surface surface = default;
        D3D12GraphicsQueue queue = default;
        D3D12Command command = default;

        if (!factory.Initialize(debug: true))
        {
            goto Error;
        }
        if (!adapter.Initialize(factory))
        {
            goto Error;
        }

        // Create the device
        var deviceArgs = new D3D12DeviceCreationArgs
        {
            Height = window.Height,
            Width = window.Width,
            WindowHandle = window.Handle,
            MinimumFeatureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0
        };

        if (!device.Initialize(adapter, deviceArgs))
        {
            goto Error;
        }

        // Create the graphics queue
        if (!queue.Initialize(device))
        {
            goto Error;
        }

        // Set up the swapchain
        var swapChainArgs = new SwapChainCreationArgs
        {
            AllowTearing = true,
            BufferCount = BufferCount,
            Height = window.Height,
            Width = window.Width,
            VSync = true,
            WindowHandle = window.Handle
        };
        if (!surface.Initialize(factory, device, queue, swapChainArgs))
        {
            goto Error;
        }

        // Create the d3d command list
        if (!command.Initialize(device, queue, BufferCount))
        {
            goto Error;
        }

        // Add the resources and register the systems needed
        builder
            .AddResource<RenderData>()
            .AddResource(new D3D12Core
            {
                Surface = surface,
                Command = command,
                Device = device,
                Queue = queue,
                Adapter = adapter
            })

            
            .AddSystemToStage<BeginFrameSystem>(Stage.PreUpdate, RunCriteria.Always)
            .AddSystemToStage<SwapChainPresentSystem>(Stage.PostUpdate, RunCriteria.Always)
            .AddShutdownSystem<D3D12TearDownSystem>(RunCriteria.Always)
            ;

        builder
            .AddModule<D3D12DebugModule>();

        
        return;

Error:
        Logger.Error<D3D12RenderModule1>("Failed to inialize the D3D12 renderer module.");
        device.Shutdown();
        surface.Shutdown();
        queue.Shutdown();
        command.Shutdown();
        adapter.Shutdown();
    }


    static void EnableDebugLayer()
    {
        // Enable the Debug layer for D3D12
        using ComPtr<ID3D12Debug> spDebugController0 = default;
        using ComPtr<ID3D12Debug1> spDebugController1 = default;
        var hr = D3D12Common.D3D12GetDebugInterface(typeof(ID3D12Debug).GUID, (void**)spDebugController0.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed {nameof(D3D12Common.D3D12GetDebugInterface)} with HRESULT: {hr}");
            return;
        }

        hr = spDebugController0.Get()->QueryInterface(typeof(ID3D12Debug1).GUID, (void**)spDebugController1.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to query {nameof(ID3D12Debug1)} interface with HRESULT: {hr}");
            return;
        }
        spDebugController1.Get()->EnableDebugLayer();
        spDebugController1.Get()->SetEnableGPUBasedValidation(true);
    }
}

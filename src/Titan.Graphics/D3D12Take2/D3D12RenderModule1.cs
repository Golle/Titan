using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;

namespace Titan.Graphics.D3D12Take2;

public unsafe struct D3D12RenderModule1 : IModule
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();

        EnableDebugLayer();

        DXGIFactory factory = default;
        DXGIAdapter adapter = default;
        D3D12Device device = default;
        D3D12Surface swapChain = default;
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

        var args = new D3D12DeviceCreationArgs
        {
            Height = window.Height,
            Width = window.Width,
            WindowHandle = window.Handle,
            MinimumFeatureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0
        };

        if (!device.Initialize(adapter, args))
        {
            goto Error;
        }
        

        if (!queue.Initialize(device))
        {
            goto Error;
        }

        var swapChainArgs = new SwapChainCreationArgs
        {
            AllowTearing = true,
            BufferCount = 3,
            Height = window.Height,
            Width = window.Width,
            VSync = true,
            WindowHandle = window.Handle
        };
        if (!swapChain.Initialize(factory, device, queue, swapChainArgs))
        {
            goto Error;
        }
        if (!command.Initialize(device, queue, 3))
        {
            goto Error;
        }

        //for (var i = 0; i < 100; ++i)
        //{
        //    swapChain.Present();
        //}

        //factory.Shutdown();
        //device.Shutdown();
        //adapter.Shutdown();
        //swapChain.Shutdown();
        //queue.Shutdown();

        builder
            .AddResource<RenderData>()
            .AddResource(new D3D12Core
            {
                Surface = swapChain,
                Command = command
            })
            ;

        builder
            .AddSystemToStage<BeginFrameSystem>(Stage.PreUpdate, RunCriteria.Always)
            .AddSystemToStage<SwapChainPresentSystem>(Stage.PostUpdate, RunCriteria.Always);
        return;




Error:
        Logger.Error<D3D12RenderModule1>("Failed to inialize the D3D12 renderer module.");
        factory.Shutdown();
        device.Shutdown();
        adapter.Shutdown();
        swapChain.Shutdown();
        queue.Shutdown();
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

internal struct D3D12Core : IResource
{
    public D3D12Surface Surface;
    public D3D12Command Command;
}

public struct RenderData : IResource
{
    public ulong FrameCount;
    public uint FrameIndex;
}



internal struct BeginFrameSystem : IStructSystem<BeginFrameSystem>
{
    private MutableResource<D3D12Core> Core;

    public static void Init(ref BeginFrameSystem system, in SystemsInitializer init)
    {
        system.Core = init.GetMutableResource<D3D12Core>();
    }

    public static void Update(ref BeginFrameSystem system)
    {
        system.Core.Get().Command.BeginFrame();
    }

    public static bool ShouldRun(in BeginFrameSystem system) => throw new System.NotImplementedException("This should be marked as always");
}

internal struct SwapChainPresentSystem : IStructSystem<SwapChainPresentSystem>
{
    private MutableResource<D3D12Core> Resources;
    private MutableResource<RenderData> RenderData;

    public static void Init(ref SwapChainPresentSystem system, in SystemsInitializer init)
    {
        system.Resources = init.GetMutableResource<D3D12Core>(false);
        system.RenderData = init.GetMutableResource<RenderData>(false);
    }

    public static void Update(ref SwapChainPresentSystem system)
    {
        ref var surface = ref system.Resources.Get().Surface;
        ref var command = ref system.Resources.Get().Command;
        ref var renderData = ref system.RenderData.Get();

        command.EndFrame();
        surface.Present();
        renderData.FrameIndex = surface.FrameIndex;
        renderData.FrameCount++;
    }
    public static bool ShouldRun(in SwapChainPresentSystem system) => throw new System.NotImplementedException("This should be marked as always");
}

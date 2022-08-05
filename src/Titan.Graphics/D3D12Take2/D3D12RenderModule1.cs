using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.Graphics.Modules;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;

namespace Titan.Graphics.D3D12Take2;

public unsafe struct D3D12RenderModule1 : IModule
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();

        DXGIFactory factory = default;
        DXGIAdapter adapter = default;
        D3D12Device device = default;
        DXGISwapChain swapChain = default;

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

        var swapChainArgs = new SwapChainCreationArgs
        {
            AllowTearing = true,
            BufferCount = 3,
            Height = window.Height,
            Width = window.Width,
            VSync = true,
            WindowHandle = window.Handle
        };
        if (!swapChain.Initialize(factory, device, swapChainArgs))
        {
            goto Error;
        }


        factory.Shutdown();
        device.Shutdown();
        adapter.Shutdown();
        swapChain.Shutdown();
        return;



// get the adapter (allow configuration for that?)
// create the device


// later: create heap allocators and copy queues
// set up 3d Renderer (Forward+)
// set up 2d Renderer (single draw call, mega texture)


// register systems for rendering and teardown

Error:
        Logger.Error<D3D12RenderModule1>("Failed to inialize the D3D12 renderer module.");
        factory.Shutdown();
        device.Shutdown();
        adapter.Shutdown();
        swapChain.Shutdown();
    }
}

internal struct D3D12Core // not sure about the name.
{
    public ComPtr<IDXGIFactory7> DXGIFactory;
    public ComPtr<IDXGIAdapter3> Adapter;
    public ComPtr<ID3D12Device4> Device;

    public ComPtr<ID3D12Debug1> DebugLayer;

}

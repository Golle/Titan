using Titan.Core;
using Titan.Graphics.D3D12;
using Titan.Systems;
using Titan.Windows;

namespace Titan.Graphics;

internal struct RenderInfoSystem : ISystem
{
    private MutableResource<RenderInfo> _renderInfo;
    private ObjectHandle<ISwapChain> _swapchain;
    private ObjectHandle<IWindow> _window;

    public void Init(in SystemInitializer init)
    {
        _renderInfo = init.GetMutableResource<RenderInfo>(false);
        _swapchain = init.GetManagedApi<ISwapChain>();
        _window = init.GetManagedApi<IWindow>();
    }

    public void Update()
    {
        var swapChain = (DXGISwapChain)_swapchain.Value;
        var window = _window.Value;
        ref var info = ref _renderInfo.Get();
        info.CurrentBackbuffer = swapChain.GetCurrentBackbuffer();
        info.BackbufferIndex = swapChain.GetBackbufferIndex();
        info.FrameCount++;
        info.WindowSize = new((int)window.Width, (int)window.Height);
    }
}

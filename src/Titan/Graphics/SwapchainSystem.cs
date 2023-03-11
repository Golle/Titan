using Titan.Core;
using Titan.Core.Logging;
using Titan.Events;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.Rendering;
using Titan.Input;
using Titan.Systems;
using Titan.Windows;
using Titan.Windows.Events;

namespace Titan.Graphics;

internal struct SwapchainSystem : ISystem
{
    private ObjectHandle<ISwapChain> _swapchain;
    private ObjectHandle<D3D12CommandQueue> _commandQueue;
    private ObjectHandle<D3D12Allocator> _allocator;
    private EventsReader<WindowSizeEvent> _windowSize;
    private InputManager _input;
    private ObjectHandle<IWindow> _window;

    public void Init(in SystemInitializer init)
    {
        _swapchain = init.GetManagedApi<ISwapChain>();
        _commandQueue = init.GetManagedApi<D3D12CommandQueue>();
        _allocator = init.GetManagedApi<D3D12Allocator>();
        _windowSize = init.GetEventsReader<WindowSizeEvent>();
        _input = init.GetInputManager();
        _window = init.GetManagedApi<IWindow>();
    }

    public void Update()
    {
        _commandQueue.Value.ExecuteAndReset();
        _swapchain.Value.Present();
        _allocator.Value.Update();

        if (_windowSize.HasEvents())
        {
            var width = 0u; 
            var height = 0u;
            foreach (ref readonly var @event in _windowSize)
            {
                width = @event.Width;
                height = @event.Height;
            }
            // find the last resize event
            Logger.Trace<SwapchainSystem>($"Resize window to {width}x{height}");
            _swapchain.Value.Resize(width, height);
        }

        if (_input.IsKeyPressed(KeyCode.Enter) && _input.IsKeyDown(KeyCode.Alt))
        {
            Logger.Trace<SwapchainSystem>($"Toggle fullscreen");
            _swapchain.Value.ToggleFullscreen();
            _swapchain.Value.Resize(_window.Value.Width, _window.Value.Height);
        }
    }
}

using System;
using Titan.Windows;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class D3D11GraphicsDevice : IGraphicsDevice
    {
        private readonly IWindow _window;
        private ComPtr<ID3D11Device> _device;
        private ComPtr<IDXGISwapChain> _swapChain;
        public IRenderContext ImmediateContext { get; private set; }
        public ID3D11Device* Ptr => _device.Get();
        public IDXGISwapChain* SwapChainPtr => _swapChain.Get();
        public ref readonly ComPtr<IDXGISwapChain> SwapChain => ref _swapChain;
        public D3D11GraphicsDevice(IWindow window) => _window = window;
        public void Initialize(uint refreshRate, bool debug = false)
        {
            var flags = debug ? 2u : 0u;
            var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
            DXGI_SWAP_CHAIN_DESC desc;
            desc.BufferCount = 2;
            //desc.BufferCount = 1;
            desc.BufferDesc.Width = (uint) _window.Width;
            desc.BufferDesc.Height = (uint) _window.Height;
            desc.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
            desc.BufferDesc.RefreshRate.Denominator = refreshRate;
            desc.BufferDesc.Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;
            desc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
            // AA
            desc.SampleDesc.Count = 1;
            desc.SampleDesc.Quality = 0;

            desc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
            desc.OutputWindow = _window.Handle;
            desc.Windowed = _window.Windowed;
            //desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;
            desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
            desc.Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;

            using var context = new ComPtr<ID3D11DeviceContext>();
            CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, context.GetAddressOf()), "D3D11CreateDeviceAndSwapChain");
            ImmediateContext = new RenderContext(context.Get());
        }

        public void ResizeBuffers()
        {
            // TODO: this will crash because RenderTargetViewManager has a reference to backbuffer. Move the backbuffer to the manager and implement resize for all buffers.
            //_backBuffer.Dispose();
            //CheckAndThrow(_swapChain.Get()->ResizeBuffers(0, 0, 0, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0), "IDXGISwapChain::ResizeBuffers");
            //InitBackBuffer(null);
        }


        public void Dispose()
        {
            (ImmediateContext as IDisposable)?.Dispose();
            _swapChain.Dispose();
            _device.Dispose();
        }
    }
}

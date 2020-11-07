using System;
using System.ComponentModel;
using Titan.Core.Memory;
using Titan.Graphics.Resources;
using Titan.Windows;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class GraphicsDevice : IGraphicsDevice
    {
        private ComPtr<ID3D11Device> _device;
        private ComPtr<IDXGISwapChain> _swapChain;
        private ComPtr<ID3D11DeviceContext> _immediateContext;
        private ComPtr<ID3D11RenderTargetView> _backBuffer;

        public IShaderResourceViewManager ShaderResourceViewManager { get; }
        public ITextureManager TextureManager { get; }
        public IVertexBufferManager VertexBufferManager { get; }
        public IIndexBufferManager IndexBufferManager { get; }
        public IConstantBufferManager ConstantBufferManager { get; }
        public IRenderTargetViewManager RenderTargetViewManager { get; }
        public IDepthStencilViewManager DepthStencilViewManager { get; }


        public ID3D11Device* Ptr => _device.Get();
        public ID3D11DeviceContext* ImmediateContextPtr => _immediateContext.Get();
        public ref readonly ComPtr<ID3D11RenderTargetView> BackBuffer => ref _backBuffer;
        public ref readonly ComPtr<IDXGISwapChain> SwapChain => ref _swapChain;

        public void ResizeBuffers()
        {
            // TODO: this will crash because RenderTargetViewManager has a reference to backbuffer. Move the backbuffer to the manager and implement resize for all buffers.
            _backBuffer.Dispose();
            CheckAndThrow(_swapChain.Get()->ResizeBuffers(0, 0, 0, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0), "IDXGISwapChain::ResizeBuffers");
            InitBackBuffer();
        }

        public GraphicsDevice(IWindow window, IMemoryManager memoryManager, uint refreshRate = 144, bool debug = true)
        {
            InitDeviceAndSwapChain(window, refreshRate, debug);
            InitBackBuffer();

            var pDevice = _device.Get();
            TextureManager = new TextureManager(pDevice, memoryManager);
            IndexBufferManager = new IndexBufferManager(pDevice, memoryManager);
            ShaderResourceViewManager = new ShaderResourceViewManager(pDevice, memoryManager);
            VertexBufferManager = new VertexBufferManager(pDevice, memoryManager);
            ConstantBufferManager = new ConstantBufferManager(pDevice, memoryManager);
            RenderTargetViewManager = new RenderTargetViewManager(pDevice, _backBuffer.Get(), memoryManager);
            DepthStencilViewManager = new DepthStencilViewManager(pDevice, memoryManager);
        }


        private void InitDeviceAndSwapChain(IWindow window, uint refreshRate, bool debug)
        {
            var flags = debug ? 2u : 0u;
            var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
            DXGI_SWAP_CHAIN_DESC desc;
            desc.BufferCount = 2;
            //desc.BufferCount = 1;
            desc.BufferDesc.Width = (uint) window.Width;
            desc.BufferDesc.Height = (uint) window.Height;
            desc.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
            desc.BufferDesc.RefreshRate.Denominator = refreshRate;
            desc.BufferDesc.Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;
            desc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
            // AA
            desc.SampleDesc.Count = 1;
            desc.SampleDesc.Quality = 0;

            desc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
            desc.OutputWindow = window.Handle;
            desc.Windowed = window.Windowed;
            //desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;
            desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
            desc.Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
            

            var result = D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _immediateContext.GetAddressOf());
            if (FAILED(result))
            {
                throw new Win32Exception(result,
                    $"Call to {nameof(D3D11CreateDeviceAndSwapChain)} failed with HRESULT {result}");
            }
        }

        private void InitBackBuffer()
        {
            ID3D11Buffer* backBuffer;
            fixed (Guid* resourcePointer = &D3D11Resource)
            {
                CheckAndThrow(_swapChain.Get()->GetBuffer(0, resourcePointer, (void**) &backBuffer), "GetBuffer");
            }
            CheckAndThrow(_device.Get()->CreateRenderTargetView((ID3D11Resource*) backBuffer, null, _backBuffer.GetAddressOf()), "CreateRenderTargetView");

            backBuffer->Release();
        }

        public void Dispose()
        {
            _backBuffer.Dispose();
            _swapChain.Dispose();
            _immediateContext.Dispose();
            _device.Dispose();
        }
    }
}

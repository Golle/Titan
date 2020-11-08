using System;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;
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

        public IShaderResourceViewManager ShaderResourceViewManager { get; }
        public ITextureManager TextureManager { get; }
        public IVertexBufferManager VertexBufferManager { get; }
        public IIndexBufferManager IndexBufferManager { get; }
        public IConstantBufferManager ConstantBufferManager { get; }
        public IRenderTargetViewManager RenderTargetViewManager { get; private set; }
        public IDepthStencilViewManager DepthStencilViewManager { get; }
        public IDepthStencilStateManager DepthStencilStateManager { get; }
        public ISamplerStateManager SamplerStateManager { get; }
        public IRenderContext ImmediateContext { get; private set; }
        public IShaderManager ShaderManager { get; }
        public ID3D11Device* Ptr => _device.Get();
        public ref readonly ComPtr<IDXGISwapChain> SwapChain => ref _swapChain;

        public void ResizeBuffers()
        {
            // TODO: this will crash because RenderTargetViewManager has a reference to backbuffer. Move the backbuffer to the manager and implement resize for all buffers.
            //_backBuffer.Dispose();
            //CheckAndThrow(_swapChain.Get()->ResizeBuffers(0, 0, 0, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0), "IDXGISwapChain::ResizeBuffers");
            //InitBackBuffer(null);
        }

        public GraphicsDevice(IWindow window, IMemoryManager memoryManager, IShaderCompiler shaderCompiler, TitanConfiguration configuration)
        {
            InitDeviceAndSwapChain(window, configuration.RefreshRate, configuration.Debug);
            InitBackBuffer(memoryManager);

            var pDevice = _device.Get();
            TextureManager = new TextureManager(pDevice, memoryManager);
            IndexBufferManager = new IndexBufferManager(pDevice, memoryManager);
            ShaderResourceViewManager = new ShaderResourceViewManager(pDevice, memoryManager);
            VertexBufferManager = new VertexBufferManager(pDevice, memoryManager);
            ConstantBufferManager = new ConstantBufferManager(pDevice, memoryManager);
            DepthStencilViewManager = new DepthStencilViewManager(pDevice, memoryManager);
            DepthStencilStateManager = new DepthStencilStateManager(pDevice, memoryManager);
            SamplerStateManager = new SamplerStateManager(pDevice, memoryManager);
            ShaderManager = new ShaderManager(pDevice, memoryManager, shaderCompiler, configuration);
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

            using var context = new ComPtr<ID3D11DeviceContext>();
            CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, context.GetAddressOf()), "D3D11CreateDeviceAndSwapChain");
            ImmediateContext = new RenderContext(context.Get());
        }

        private void InitBackBuffer(IMemoryManager memoryManager)
        {
            using var renderTarget = new ComPtr<ID3D11RenderTargetView>();
            using var backbuffer = new ComPtr<ID3D11Buffer>();
            fixed (Guid* resourcePointer = &D3D11Resource)
            {
                CheckAndThrow(_swapChain.Get()->GetBuffer(0, resourcePointer, (void**) backbuffer.GetAddressOf()), "GetBuffer");
            }
            CheckAndThrow(_device.Get()->CreateRenderTargetView((ID3D11Resource*) backbuffer.Get(), null, renderTarget.GetAddressOf()), "CreateRenderTargetView");
            
            RenderTargetViewManager = new RenderTargetViewManager(_device.Get(), renderTarget.Get(), memoryManager);
        }

        public void Dispose()
        {
            TextureManager.Dispose();
            IndexBufferManager.Dispose();
            ShaderResourceViewManager.Dispose();
            VertexBufferManager.Dispose();
            ConstantBufferManager.Dispose();
            RenderTargetViewManager.Dispose();
            DepthStencilViewManager.Dispose();
            DepthStencilStateManager.Dispose();
            SamplerStateManager.Dispose();
            ShaderManager.Dispose();
            (ImmediateContext as IDisposable)?.Dispose();

            _swapChain.Dispose();
            _device.Dispose();
        }
    }
}

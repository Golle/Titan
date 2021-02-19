using System;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.GraphicsV2.D3D11
{
    public record DeviceConfiguration(HWND WindowHandle, uint Width, uint Height, uint RefreshRate, bool Windowed, bool VSync, bool Debug = false);

    internal unsafe class Device : IDisposable
    {
        // D3d pointers
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11DeviceContext> _context;
        private ComPtr<IDXGISwapChain> _swapChain;

        // Swapchain and Context
        public Swapchain Swapchain { get; }
        public Context Context { get; }
        public TextureManager TextureManager { get; }
        public BufferManager BufferManager { get; }
        public ShaderManager ShaderManager { get; }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Device* Get() => _device.Get();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11DeviceContext* GetContext() => _context.Get();
        
        internal Device(DeviceConfiguration configuration)
        {
            var flags = configuration.Debug ? 2u : 0u;
            var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
            var desc = new DXGI_SWAP_CHAIN_DESC
            {
                BufferCount = 2,
                BufferDesc = new DXGI_MODE_DESC
                {
                    Width = configuration.Width,
                    Height = configuration.Height,
                    RefreshRate = new DXGI_RATIONAL { Denominator = configuration.RefreshRate },
                    Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED,
                    ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
                    Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM
                },
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                },
                BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
                OutputWindow = configuration.WindowHandle,
                SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
                Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH,
                Windowed = configuration.Windowed
            };

            LOGGER.Debug("Creating D3D11 Device");
            CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _context.GetAddressOf()), nameof(D3D11CreateDeviceAndSwapChain));
            LOGGER.Debug("D3D11 Device Created");

            Context = new Context(_context.Get());
            Swapchain = new Swapchain(_swapChain.Get(), configuration.VSync, configuration.Width, configuration.Height);

            TextureManager = new TextureManager(this, Swapchain);
            BufferManager = new BufferManager(this);
            ShaderManager = new ShaderManager(this);
        }


        
        public void Dispose()
        {
            TextureManager.Dispose();
            BufferManager.Dispose();
            ShaderManager.Dispose();

            _context.Dispose();
            _swapChain.Dispose();
            _device.Dispose();
        }
    }
    
}

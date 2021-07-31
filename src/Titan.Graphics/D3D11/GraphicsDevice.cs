using System;
using Titan.Core.Logging;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Rasterizer;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Windows;
using Titan.Windows;
using Titan.Windows.D3D11;
using static Titan.Windows.Common;
using static Titan.Windows.D3D11.D3D11Common;
using static Titan.Windows.D3D11.DXGI_FORMAT;
using static Titan.Windows.D3D11.DXGI_MODE_SCALING;
using static Titan.Windows.D3D11.DXGI_MODE_SCANLINE_ORDER;
using static Titan.Windows.D3D11.DXGI_SWAP_CHAIN_FLAG;
using static Titan.Windows.D3D11.DXGI_SWAP_EFFECT;
using static Titan.Windows.D3D11.DXGI_USAGE;

namespace Titan.Graphics.D3D11
{
    public record DeviceConfiguration(uint RefreshRate, bool Vsync, bool Debug, bool Stats);

    public static class GraphicsDevice
    {
        private static ComPtr<ID3D11Device> _device;
        private static ComPtr<ID3D11DeviceContext> _context;
        private static ComPtr<IDXGISwapChain> _swapChain;
        private static ComPtr<ID3D11RenderTargetView> _backbuffer;


        public static SwapChain SwapChain { get; private set; }
        public static Context ImmediateContext { get; private set; }
        public static bool IsInitialized { get; private set; }

        public static BufferManager BufferManager { get; private set; }
        public static TextureManager TextureManager { get; private set; }
        public static SamplerManager SamplerManager { get; private set; }
        public static ShaderManager ShaderManager { get; private set; }
        public static RasterizerManager RasterizerManager { get; private set; }
        public static BlendStateManager BlendStateManager { get; private set; }

        public static void Init(Window window, DeviceConfiguration config)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(GraphicsDevice)} has already been initialized.");
            }

            var deviceCreationFlags = D3D11_CREATE_DEVICE_FLAG.UNSPECIFIED;
            if (config.Debug)
            {
                deviceCreationFlags |= D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;
            }

            if (config.Stats)
            {
                deviceCreationFlags |= D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;
            }

            var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
            var desc = new DXGI_SWAP_CHAIN_DESC
            {
                BufferCount = 2,
                BufferDesc = new DXGI_MODE_DESC
                {
                    Width = window.Width,
                    Height = window.Height,
                    RefreshRate = new DXGI_RATIONAL { Denominator = config.RefreshRate },
                    Scaling = DXGI_MODE_SCALING_UNSPECIFIED,
                    ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
                    Format = DXGI_FORMAT_B8G8R8A8_UNORM
                },
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                },

                BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
                OutputWindow = window.Handle,
                SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
                Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH,
                Windowed = window.Windowed
            };

            Logger.Trace<ID3D11Device>("Creating device");
            unsafe
            {
                CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, deviceCreationFlags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _context.GetAddressOf()), nameof(D3D11CreateDeviceAndSwapChain));
            }
            Logger.Trace<ID3D11Device>("Device created");

            // Get the backbuffer
            Logger.Trace<ID3D11Device>($"Creating Backbuffer ({D3D11Resource})");
            using var backbufferResource = new ComPtr<ID3D11Resource>();
            unsafe
            {
                fixed (Guid* resourcePointer = &D3D11Resource)
                {
                    CheckAndThrow(_swapChain.Get()->GetBuffer(0, resourcePointer, (void**)backbufferResource.GetAddressOf()), nameof(IDXGISwapChain.GetBuffer));
                }
                CheckAndThrow(_device.Get()->CreateRenderTargetView(backbufferResource.Get(), null, _backbuffer.GetAddressOf()), nameof(ID3D11Device.CreateRenderTargetView));
            }
            Logger.Trace<ID3D11Device>("Backbuffer created");


            unsafe
            {
                ImmediateContext = new Context(_context.Get());
                SwapChain = new SwapChain(_swapChain.Get(), _backbuffer.Get(), config.Vsync, window.Width, window.Height);

                BufferManager = new BufferManager(_device);
                TextureManager = new TextureManager(_device.Get(), SwapChain);
                SamplerManager = new SamplerManager(_device.Get());
                ShaderManager = new ShaderManager(_device.Get());
                RasterizerManager = new RasterizerManager(_device.Get());
                BlendStateManager = new BlendStateManager(_device.Get());
            }

            IsInitialized = true;
        }


        public static void Terminate()
        {
            if (IsInitialized)
            {
                Logger.Trace<ID3D11Device>("Disposing managers");
                BufferManager.Dispose();
                BufferManager = null;
                TextureManager.Dispose();
                TextureManager = null;
                SamplerManager.Dispose();
                SamplerManager = null;
                ShaderManager.Dispose();
                ShaderManager = null;
                RasterizerManager.Dispose();
                RasterizerManager = null;

                Logger.Trace<ID3D11Device>("Disposing resources");
                _backbuffer.Dispose();
                _swapChain.Dispose();
                _context.Dispose();
                _device.Dispose();
                
            }
            IsInitialized = false;
        }
    }
}

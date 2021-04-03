using System;
using Titan.Core.Logging;
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
    public record DeviceConfiguration(uint RefreshRate, bool Vsync, bool Debug);

    public static class GraphicsDevice
    {
        private static ComPtr<ID3D11Device> _device;
        private static ComPtr<ID3D11DeviceContext> _context;
        private static ComPtr<IDXGISwapChain> _swapChain;
        private static ComPtr<ID3D11RenderTargetView> _backbuffer;

        private static bool _initialized;
        public static void Init(Window window, DeviceConfiguration config)
        {
            if (_initialized)
            {
                throw new InvalidOperationException($"{nameof(GraphicsDevice)} has already been initialized.");
            }

            var flags = config.Debug ? 2u : 0u;
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
                    Format = DXGI_FORMAT_R8G8B8A8_UNORM
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
                CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _context.GetAddressOf()), nameof(D3D11CreateDeviceAndSwapChain));
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

            _initialized = true;
        }


        public static void Terminate()
        {
            
            if (_initialized)
            {
                Logger.Trace<ID3D11Device>("Disposing resources");
                _backbuffer.Dispose();
                _swapChain.Dispose();
                _context.Dispose();
                _device.Dispose();
            }
            _initialized = false;
        }
    }
}

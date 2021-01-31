using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.GraphicsV2.D3D11
{

    public record DeviceConfiguration(HWND WindowHandle, uint Width, uint Height, uint RefreshRate, bool Windowed, bool Debug = false);
    internal unsafe class Device : IDisposable
    {
        private ComPtr<ID3D11Device> _device;
        private ComPtr<IDXGISwapChain> _swapChain;
        private ComPtr<ID3D11DeviceContext> _context;
        public void Initialize(DeviceConfiguration configuration)
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
                    RefreshRate = new DXGI_RATIONAL { Denominator = configuration.RefreshRate},
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
            CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _context.GetAddressOf()), nameof(D3D11CreateDeviceAndSwapChain));
        }

        public void Dispose()
        {
            _swapChain.Dispose();
            _context.Dispose();
            _device.Dispose();
        }
    }
}

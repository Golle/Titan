using System.ComponentModel;
using Titan.Windows;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class D3D11GraphicsDevice : ID3D11GraphicsDevice
    {
        ID3D11Device* ID3D11GraphicsDevice.Ptr => _device.Get();
        ID3D11DeviceContext* ID3D11GraphicsDevice.ImmediateContextPtr => _immediateContext.Get();

        private ComPtr<ID3D11Device> _device;
        private ComPtr<IDXGISwapChain> _swapChain;
        private ComPtr<ID3D11DeviceContext> _immediateContext;

        public D3D11GraphicsDevice(IWindow window, uint refreshRate = 144, bool debug = true)
        {
            var flags = debug ? 2u : 0u;
            var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
            DXGI_SWAP_CHAIN_DESC desc;
            desc.BufferCount = 1;
            desc.BufferDesc.Width = (uint) window.Width;
            desc.BufferDesc.Height = (uint) window.Height;
            desc.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
            desc.BufferDesc.RefreshRate.Denominator = refreshRate;
            desc.BufferDesc.Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;
            desc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
            // AA
            desc.SampleDesc.Count = 1;
            desc.SampleDesc.Quality = 0;

            desc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
            desc.OutputWindow = window.Handle;
            desc.Windowed = window.Windowed;
            desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;
            desc.Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;

            var result = D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, &featureLevel, 1, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _immediateContext.GetAddressOf());
            if (FAILED(result))
            {
                throw new Win32Exception(result, $"Call to {nameof(D3D11CreateDeviceAndSwapChain)} failed with HRESULT {result}");
            }
        }

        public void Dispose()
        {
            _swapChain.Dispose();
            _immediateContext.Dispose();
            _device.Dispose();
        }
    }
}

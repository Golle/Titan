using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.GraphicsV2.D3D11
{
    public unsafe class RenderTargetViewFactory
    {
        private readonly Device _device;
        private readonly Swapchain _swapchain;

        internal RenderTargetViewFactory(Device device, Swapchain swapchain)
        {
            _device = device;
            _swapchain = swapchain;
        }

        internal ID3D11RenderTargetView* CreateBackbuffer()
        {
            using var backbufferResource = new ComPtr<ID3D11Resource>();
            fixed (Guid* resourcePointer = &D3D11Common.D3D11Resource)
            {
                CheckAndThrow(_swapchain.Get()->GetBuffer(0, resourcePointer, (void**)backbufferResource.GetAddressOf()), nameof(IDXGISwapChain.GetBuffer));
            }
            ID3D11RenderTargetView* backbuffer;
            CheckAndThrow(_device.Get()->CreateRenderTargetView(backbufferResource.Get(), null, &backbuffer), nameof(ID3D11Device.CreateRenderTargetView));
            return backbuffer;
        }

        internal ID3D11RenderTargetView* Create(in Texture2D texture) => Create(texture, texture.Format);
        internal ID3D11RenderTargetView* Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_RENDER_TARGET_VIEW_DESC
            {
                Format = format,
                Texture2D = new D3D11_TEX2D_RTV
                {
                    MipSlice = 0
                },
                ViewDimension = D3D11_RTV_DIMENSION.D3D11_RTV_DIMENSION_TEXTURE2D
            };
            ID3D11RenderTargetView* renderTargetView;
            CheckAndThrow(_device.Get()->CreateRenderTargetView(resource, &desc, &renderTargetView), nameof(ID3D11Device.CreateRenderTargetView));
            return renderTargetView;
        }
    }
}

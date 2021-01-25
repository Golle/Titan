using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11_RTV_DIMENSION;

namespace Titan.Graphics.D3D11.RenderTarget
{
    internal unsafe class RenderTargetViewFactory : IDisposable
    {
        private ComPtr<ID3D11Device> _device;

        public RenderTargetViewFactory(D3D11GraphicsDevice device)
        {
            _device = new ComPtr<ID3D11Device>(device.Ptr);
        }

        public RenderTargetView Create(ID3D11Resource* pResource, DXGI_FORMAT format)
        {
            var desc = new D3D11_RENDER_TARGET_VIEW_DESC
            {
                Format = format,
                Texture2D = new D3D11_TEX2D_RTV
                {
                    MipSlice = 0
                },
                ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D
            };
            ID3D11RenderTargetView* pRenderTargetView;
            Common.CheckAndThrow(_device.Get()->CreateRenderTargetView(pResource, &desc, &pRenderTargetView), nameof(ID3D11Device.CreateRenderTargetView));
            return new RenderTargetView(pRenderTargetView, pResource);
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}

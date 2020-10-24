using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class DepthStencilView : IDisposable
    {
        public ref readonly ComPtr<ID3D11DepthStencilView> Ptr => ref _depthStencil;
        private ComPtr<ID3D11DepthStencilView> _depthStencil;
        public DepthStencilView(IGraphicsDevice device, IResource resource)
        {
            D3D11_DEPTH_STENCIL_VIEW_DESC desc = default;
            desc.Texture2D.MipSlice = 0;
            //desc.Format = resource.Format;
            desc.Format = DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT;
            desc.ViewDimension = D3D11_DSV_DIMENSION.D3D11_DSV_DIMENSION_TEXTURE2D;

            CheckAndThrow(device.Ptr->CreateDepthStencilView(resource.AsResourcePointer(), &desc, _depthStencil.GetAddressOf()), "CreateDepthStencilView");
        }

        public void Dispose() => _depthStencil.Dispose();
    }
}

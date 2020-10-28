using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
namespace Titan.Graphics.D3D11
{
    public unsafe class RenderTargetView : IDisposable
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ID3D11RenderTargetView* AsPointer() => _renderTargetView.Get();

        private ComPtr<ID3D11RenderTargetView> _renderTargetView;
        internal ref readonly ComPtr<ID3D11RenderTargetView> Ptr => ref _renderTargetView;

        public RenderTargetView(IGraphicsDevice device, IResource resource)
        {
            D3D11_RENDER_TARGET_VIEW_DESC desc = default;
            desc.Texture2D.MipSlice = 0;
            desc.Format = resource.Format;//DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
            desc.ViewDimension = D3D11_RTV_DIMENSION.D3D11_RTV_DIMENSION_TEXTURE2D;
            
            CheckAndThrow(device.Ptr->CreateRenderTargetView(resource.AsResourcePointer(), &desc, _renderTargetView.GetAddressOf()), "CreateRenderTargetView");
        }

        protected RenderTargetView(ID3D11RenderTargetView* renderTargetView)
        {
            _renderTargetView = new ComPtr<ID3D11RenderTargetView>(renderTargetView);
        }

        public void Dispose() => _renderTargetView.Dispose();
    }
}

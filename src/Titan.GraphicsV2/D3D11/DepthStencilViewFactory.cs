using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class DepthStencilViewFactory
    {
        private readonly Device _device;

        internal DepthStencilViewFactory(Device device)
        {
            _device = device;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DepthStencilView Create(in Texture2D texture2D) => Create(texture2D, texture2D.Format);
        public DepthStencilView Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_DEPTH_STENCIL_VIEW_DESC
            {
                Texture2D = new D3D11_TEX2D_DSV
                {
                    MipSlice = 0
                },
                Flags = 0,
                Format = format,
                ViewDimension = D3D11_DSV_DIMENSION.D3D11_DSV_DIMENSION_TEXTURE2D
            };

            ID3D11DepthStencilView* depthStencilView;
            CheckAndThrow(_device.Get()->CreateDepthStencilView(resource, &desc, &depthStencilView), nameof(ID3D11Device.CreateDepthStencilView));
            return new(depthStencilView);
        }
    }

    internal readonly unsafe struct DepthStencilView
    {
        private readonly ID3D11DepthStencilView* _view;
        public DepthStencilView(ID3D11DepthStencilView* view)
        {
            _view = view;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11DepthStencilView* AsPointer() => _view;
    }
}

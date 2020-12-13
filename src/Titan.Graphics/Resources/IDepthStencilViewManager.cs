using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IDepthStencilViewManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        unsafe DepthStencilViewHandle Create(ID3D11Resource* resource, DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT);
        void Destroy(in DepthStencilViewHandle handle);
        ref readonly DepthStencilView this[in DepthStencilViewHandle handle] { get; }
    }
}

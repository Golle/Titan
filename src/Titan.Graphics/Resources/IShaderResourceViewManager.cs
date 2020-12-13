using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IShaderResourceViewManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        unsafe ShaderResourceViewHandle Create(ID3D11Resource* resource, DXGI_FORMAT format);
        void Destroy(in ShaderResourceViewHandle handle);
        ref readonly ShaderResourceView this[in ShaderResourceViewHandle handle] { get; }
    }
}

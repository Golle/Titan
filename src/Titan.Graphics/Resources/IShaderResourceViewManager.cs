using System;
using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IShaderResourceViewManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        unsafe Handle<ShaderResourceView> Create(ID3D11Resource* resource, DXGI_FORMAT format);
        void Destroy(in Handle<ShaderResourceView> handle);
        ref readonly ShaderResourceView this[in Handle<ShaderResourceView> handle] { get; }
    }
}

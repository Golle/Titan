using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface ITexture2DManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        
        Handle<Texture2D> CreateTexture(uint width, uint height, DXGI_FORMAT format, D3D11_BIND_FLAG bindFlag = default);
        unsafe Handle<Texture2D> CreateTexture(uint width, uint height, DXGI_FORMAT format, void* buffer, uint stride, D3D11_BIND_FLAG bindFlag = default);
        void Destroy(in Handle<Texture2D> handle);
        ref readonly Texture2D this[in Handle<Texture2D> handle] { get; }
    }
}

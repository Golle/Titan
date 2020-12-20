using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface ITextureManager : IDisposable
    {

        void Initialize(IGraphicsDevice graphicsDevice);
        
        TextureHandle CreateTexture(uint width, uint height, DXGI_FORMAT format, D3D11_BIND_FLAG bindFlag = default);
        unsafe TextureHandle CreateTexture(uint width, uint height, DXGI_FORMAT format, void* buffer, uint stride, D3D11_BIND_FLAG bindFlag = default);
        void Destroy(in TextureHandle textureHandle);
        ref readonly Texture this[in TextureHandle textureHandle] { get; }
    }
}

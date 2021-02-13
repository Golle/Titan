using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering
{
    internal class DepthStencil : IDisposable
    {
        public DXGI_FORMAT Format { get; }
        public uint Width { get; }
        public uint Height { get; }

        private ComPtr<ID3D11Texture2D> _texture;
        private ComPtr<ID3D11DepthStencilView> _stencil;

        public unsafe DepthStencil(ID3D11Texture2D* texture2d, ID3D11DepthStencilView* depthStencil, DXGI_FORMAT format, uint width, uint height)
        {
            Format = format;
            Width = width;
            Height = height;
            _texture = new ComPtr<ID3D11Texture2D>(texture2d);
            _stencil = new ComPtr<ID3D11DepthStencilView>(depthStencil);
        }

        public void Dispose()
        {
            _stencil.Dispose();
            _texture.Dispose();
        }
    }
}

using System;
using Titan.GraphicsV2.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering
{
    internal class FrameBufferTexture : IDisposable
    {
        public string Name { get; }
        public DXGI_FORMAT Format { get; }
        public uint Height { get; }
        public uint Width { get; }
        public bool Clear { get; }
        public Color Color { get; }

        internal unsafe ID3D11RenderTargetView* RenderTargetView => _renderTargetView.Get();
        internal unsafe ID3D11ShaderResourceView* ShaderResourceView => _shaderResourceView.Get();

        private ComPtr<ID3D11Texture2D> _texture;
        private ComPtr<ID3D11ShaderResourceView> _shaderResourceView;
        private ComPtr<ID3D11RenderTargetView> _renderTargetView;

        public unsafe FrameBufferTexture(string name, ID3D11Texture2D* texture, ID3D11ShaderResourceView* shaderResourceView, ID3D11RenderTargetView* renderTargetView, DXGI_FORMAT format, uint width, uint height, bool clear, in Color color)
        {
            Name = name;
            Format = format;
            Width = width;
            Height = height;
            Clear = clear;
            Color = color;
            _texture = ComPtr<ID3D11Texture2D>.Wrap(texture);
            _renderTargetView = ComPtr<ID3D11RenderTargetView>.Wrap(renderTargetView);
            _shaderResourceView = ComPtr<ID3D11ShaderResourceView>.Wrap(shaderResourceView);
        }

        public void Dispose()
        {
            _shaderResourceView.Dispose();
            _renderTargetView.Dispose();
            _texture.Dispose();
        }
    }
}

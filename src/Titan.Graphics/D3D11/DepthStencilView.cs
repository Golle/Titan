using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class DepthStencilView : IDisposable
    {
        public ref readonly ComPtr<ID3D11DepthStencilView> Ptr => ref _depthStencil;
        private ComPtr<ID3D11DepthStencilView> _depthStencil;
        public DepthStencilView(IGraphicsDevice device)
        {
            // TODO: add implementation
        }

        public void Dispose()
        {
        }
    }
}

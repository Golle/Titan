using System;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{

    public unsafe class ImmediateContext : IDisposable
    {
        private ComPtr<ID3D11DeviceContext> _context;

        public ImmediateContext(IGraphicsDevice device)
        {
            _context = new ComPtr<ID3D11DeviceContext>(device.ImmediateContextPtr);
        }

        public void SetVertexBuffer(IVertexBuffer vertexBuffer, uint slot = 0u, uint offset = 0u)
        {
            var stride = vertexBuffer.Stride;
            _context.Get()->IASetVertexBuffers(slot, 1, vertexBuffer.Buffer.GetAddressOf(), &stride, &offset);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public interface IGraphicsDevice : IDisposable
    {

        internal unsafe ID3D11Device* Ptr { get; }
        internal unsafe ID3D11DeviceContext* ImmediateContextPtr { get; }
        internal ref readonly ComPtr<ID3D11RenderTargetView> BackBuffer { get; }
        internal ref readonly ComPtr<IDXGISwapChain> SwapChain { get; }
    }
}

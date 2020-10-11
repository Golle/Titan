using System;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargetView(RenderTargetView renderTargetView, in Color color)
        {
            fixed (Color* ptr = &color)
            {
                _context.Get()->ClearRenderTargetView(renderTargetView.Ptr.Get(), (float*)ptr);
            }
        }

        // TODO: is this the best way to do it? 
        public void SetVertexBuffer(IVertexBuffer vertexBuffer, uint slot = 0u, uint offset = 0u)
        {
            var stride = vertexBuffer.Stride;
            _context.Get()->IASetVertexBuffers(slot, 1, vertexBuffer.Buffer.GetAddressOf(), &stride, &offset);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY topology) => _context.Get()->IASetPrimitiveTopology(topology);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInputLayout(InputLayout inputLayout) => _context.Get()->IASetInputLayout(inputLayout.Pointer.Get());
        public void Dispose() => _context.Dispose();
    }
}

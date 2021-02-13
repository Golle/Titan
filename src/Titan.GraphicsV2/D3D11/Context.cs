using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Context : IDisposable
    {
        private ComPtr<ID3D11DeviceContext> _context;
        public Context(ID3D11DeviceContext* context)
        {
            _context = new ComPtr<ID3D11DeviceContext>(context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(uint vertexCount, uint startVertexLocation) => _context.Get()->Draw(vertexCount, startVertexLocation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint indexCount, uint startIndex = 0u, int baseVertexIndex = 0) => _context.Get()->DrawIndexed(indexCount, startIndex, baseVertexIndex);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(uint count, ID3D11RenderTargetView** renderTarget, ID3D11DepthStencilView* depthStencilView) => _context.Get()->OMSetRenderTargets(count, renderTarget, depthStencilView);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTarget(ID3D11RenderTargetView* renderTarget, in Color color)
        {
            fixed (Color* pColor = &color)
            {
                _context.Get()->ClearRenderTargetView(renderTarget, (float*) pColor);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

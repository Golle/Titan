using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Context
    {
        public readonly ID3D11DeviceContext* _context;
        public Context(ID3D11DeviceContext* context)
        {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(uint vertexCount, uint startVertexLocation) => _context->Draw(vertexCount, startVertexLocation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint indexCount, uint startIndex = 0u, int baseVertexIndex = 0) => _context->DrawIndexed(indexCount, startIndex, baseVertexIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(uint count, ID3D11RenderTargetView** renderTarget, ID3D11DepthStencilView* depthStencilView) => _context->OMSetRenderTargets(count, renderTarget, depthStencilView);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTarget(ID3D11RenderTargetView* renderTarget, float* color) => _context->ClearRenderTargetView(renderTarget, color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResources(uint numberOfViews, ID3D11ShaderResourceView** resourceViews) => _context->PSSetShaderResources(0, numberOfViews, resourceViews);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResources(uint numberOfViews, ID3D11ShaderResourceView** resourceViews) => _context->VSSetShaderResources(0, numberOfViews, resourceViews);
    }
}

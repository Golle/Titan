using System.Runtime.CompilerServices;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11
{
    public readonly unsafe struct Context
    {
        private readonly ID3D11DeviceContext* _context;
        public Context(ID3D11DeviceContext* context)
        {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTarget(ID3D11RenderTargetView* renderTargetView, float* color)
        {
            _context->ClearRenderTargetView(renderTargetView, color);
        }
    }
}

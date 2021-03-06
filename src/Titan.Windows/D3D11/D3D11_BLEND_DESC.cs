using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11
{
    public struct D3D11_BLEND_DESC
    {
        public int AlphaToCoverageEnable;
        public int IndependentBlendEnable;
        public D3D11_BLEND_DESC_FIXED RenderTarget;

        public struct D3D11_BLEND_DESC_FIXED
        {
            public D3D11_RENDER_TARGET_BLEND_DESC desc0;
            public D3D11_RENDER_TARGET_BLEND_DESC desc1;
            public D3D11_RENDER_TARGET_BLEND_DESC desc2;
            public D3D11_RENDER_TARGET_BLEND_DESC desc3;
            public D3D11_RENDER_TARGET_BLEND_DESC desc4;
            public D3D11_RENDER_TARGET_BLEND_DESC desc5;
            public D3D11_RENDER_TARGET_BLEND_DESC desc6;
            public D3D11_RENDER_TARGET_BLEND_DESC desc7;

            public ref D3D11_RENDER_TARGET_BLEND_DESC this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref MemoryMarshal.CreateSpan(ref desc0, 8)[index];
            }
        }
    }
}

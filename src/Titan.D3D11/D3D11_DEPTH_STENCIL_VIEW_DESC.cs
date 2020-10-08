using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.D3D11
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_DEPTH_STENCIL_VIEW_DESC
    {
        public DXGI_FORMAT Format;
        public D3D11_DSV_DIMENSION ViewDimension;
        public uint Flags;

        private D3D11_DEPTH_STENCIL_VIEW_DESC_UNION UnionMembers;
        
        public ref D3D11_TEX2D_DSV Texture2D
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref UnionMembers.Texture2D, 1));
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct D3D11_DEPTH_STENCIL_VIEW_DESC_UNION
        {
            //public D3D11_TEX1D_DSV Texture1D;
            //public D3D11_TEX1D_ARRAY_DSV Texture1DArray;
            [FieldOffset(0)]
            public D3D11_TEX2D_DSV Texture2D;
            //public D3D11_TEX2D_ARRAY_DSV Texture2DArray;
            //public D3D11_TEX2DMS_DSV Texture2DMS;
            //public D3D11_TEX2DMS_ARRAY_DSV Texture2DMSArray;
        }
    }
}

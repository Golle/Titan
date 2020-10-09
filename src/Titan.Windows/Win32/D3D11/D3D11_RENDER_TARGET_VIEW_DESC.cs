using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.D3D11
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_RENDER_TARGET_VIEW_DESC
    {
        public DXGI_FORMAT Format;
        public D3D11_RTV_DIMENSION ViewDimension;
        private D3D11_RENDER_TARGET_VIEW_DESC_UNION UnionMembers;
        public ref D3D11_TEX2D_RTV Texture2D => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref UnionMembers.Texture2D, 1));
        [StructLayout(LayoutKind.Explicit)]
        private struct D3D11_RENDER_TARGET_VIEW_DESC_UNION
        {
            //public D3D11_BUFFER_RTV Buffer;
            //public D3D11_TEX1D_RTV Texture1D;
            //public D3D11_TEX1D_ARRAY_RTV Texture1DArray;
            [FieldOffset(0)]
            public D3D11_TEX2D_RTV Texture2D;
            //public D3D11_TEX2D_ARRAY_RTV Texture2DArray;
            //public D3D11_TEX2DMS_RTV Texture2DMS;
            //public D3D11_TEX2DMS_ARRAY_RTV Texture2DMSArray;
            //public D3D11_TEX3D_RTV Texture3D;
        } 
    }
}

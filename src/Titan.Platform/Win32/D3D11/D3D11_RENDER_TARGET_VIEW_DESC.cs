using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.D3D11;

[StructLayout(LayoutKind.Sequential)]
public struct D3D11_RENDER_TARGET_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D11_RTV_DIMENSION ViewDimension;
    private D3D11_RENDER_TARGET_VIEW_DESC_UNION UnionMembers;
    public unsafe ref D3D11_TEX2D_RTV Texture2D
    {
        get
        {
            fixed (D3D11_RENDER_TARGET_VIEW_DESC_UNION* ptr = &UnionMembers)
            {
                return ref ptr->Texture2D;
            }
        }
    }

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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.D3D;
using Titan.Windows.DXGI;

namespace Titan.Windows.D3D11;

[StructLayout(LayoutKind.Sequential)]
public struct D3D11_SHADER_RESOURCE_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D_SRV_DIMENSION ViewDimension;
    private D3D11_SHADER_RESOURCE_VIEW_DESC_UNION UnionMembers;
    //public unsafe ref D3D11_TEX2D_SRV Texture2D // this generates a lot more code.
    //{
    //    get
    //    {
    //        fixed (D3D11_SHADER_RESOURCE_VIEW_DESC_UNION* ptr = &UnionMembers)
    //        {
    //            return ref ptr->Texture2D;
    //        }
    //    }
    //}
    public unsafe ref D3D11_TEX2D_SRV Texture2D => ref ((D3D11_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref UnionMembers))->Texture2D;

    [StructLayout(LayoutKind.Explicit)]
    internal struct D3D11_SHADER_RESOURCE_VIEW_DESC_UNION
    {
        [FieldOffset(0)]
        //D3D11_BUFFER_SRV Buffer;
        //D3D11_TEX1D_SRV Texture1D;
        //D3D11_TEX1D_ARRAY_SRV Texture1DArray;
        public D3D11_TEX2D_SRV Texture2D; // only one we use at the moment
        //D3D11_TEX2D_ARRAY_SRV Texture2DArray;
        //D3D11_TEX2DMS_SRV Texture2DMS;
        //D3D11_TEX2DMS_ARRAY_SRV Texture2DMSArray;
        //D3D11_TEX3D_SRV Texture3D;
        //D3D11_TEXCUBE_SRV TextureCube;
        //D3D11_TEXCUBE_ARRAY_SRV TextureCubeArray;
        //D3D11_BUFFEREX_SRV BufferEx;    
    }
}

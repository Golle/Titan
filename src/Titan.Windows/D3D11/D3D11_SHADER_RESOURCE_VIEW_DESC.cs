using System.Runtime.InteropServices;
using Titan.Windows.D3D;
using Titan.Windows.DXGI;

namespace Titan.Windows.D3D11;

[StructLayout(LayoutKind.Explicit)]
public struct D3D11_SHADER_RESOURCE_VIEW_DESC
{
    private const int UnionOffset = sizeof(DXGI_FORMAT) + sizeof(D3D_SRV_DIMENSION);
    [FieldOffset(0)]
    public DXGI_FORMAT Format;

    [FieldOffset(sizeof(DXGI_FORMAT))]
    public D3D_SRV_DIMENSION ViewDimension;

    [FieldOffset(UnionOffset)]
    public D3D11_BUFFER_SRV Buffer;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEX1D_SRV Texture1D;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEX1D_ARRAY_SRV Texture1DArray;
    [FieldOffset(UnionOffset)]
    public D3D11_TEX2D_SRV Texture2D; // only one we use at the moment
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEX2D_ARRAY_SRV Texture2DArray;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEX2DMS_SRV Texture2DMS;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEX2DMS_ARRAY_SRV Texture2DMSArray;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEX3D_SRV Texture3D;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEXCUBE_SRV TextureCube;
    //[FieldOffset(UnionOffset)]
    //public D3D11_TEXCUBE_ARRAY_SRV TextureCubeArray;
    //[FieldOffset(UnionOffset)]
    //public D3D11_BUFFEREX_SRV BufferEx;    
}

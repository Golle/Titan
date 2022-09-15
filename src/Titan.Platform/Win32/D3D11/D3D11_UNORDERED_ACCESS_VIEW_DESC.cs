using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D11;

[StructLayout(LayoutKind.Explicit)]
public struct D3D11_UNORDERED_ACCESS_VIEW_DESC
{
    private const int UnionOffset = sizeof(DXGI_FORMAT) + sizeof(D3D11_UAV_DIMENSION);

    [FieldOffset(0)]
    public DXGI_FORMAT Format;
    [FieldOffset(sizeof(DXGI_FORMAT))]
    public D3D11_UAV_DIMENSION ViewDimension;

    // TODO: Add more structs when needed
    [FieldOffset(UnionOffset)]
    public D3D11_BUFFER_UAV Buffer;
    //D3D11_TEX1D_UAV Texture1D;
    //D3D11_TEX1D_ARRAY_UAV Texture1DArray;
    //D3D11_TEX2D_UAV Texture2D;
    //D3D11_TEX2D_ARRAY_UAV Texture2DArray;
    //D3D11_TEX3D_UAV Texture3D;
}

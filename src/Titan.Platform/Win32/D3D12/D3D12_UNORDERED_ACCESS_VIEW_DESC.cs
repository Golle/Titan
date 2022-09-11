using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_UNORDERED_ACCESS_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D12_UAV_DIMENSION ViewDimension;
    private D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION _union;
    public ref D3D12_BUFFER_UAV Buffer => ref ((D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Buffer;
    public ref D3D12_TEX1D_UAV Texture1D => ref ((D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture1D;
    public ref D3D12_TEX1D_ARRAY_UAV Texture1DArray => ref ((D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture1DArray;
    public ref D3D12_TEX2D_UAV Texture2D => ref ((D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2D;
    public ref D3D12_TEX2D_ARRAY_UAV Texture2DArray => ref ((D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DArray;
    public ref D3D12_TEX3D_UAV Texture3D => ref ((D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture3D;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_UNORDERED_ACCESS_VIEW_DESC_UNION
    {
        [FieldOffset(0)]
        public D3D12_BUFFER_UAV Buffer;
        [FieldOffset(0)]
        public D3D12_TEX1D_UAV Texture1D;
        [FieldOffset(0)]
        public D3D12_TEX1D_ARRAY_UAV Texture1DArray;
        [FieldOffset(0)]
        public D3D12_TEX2D_UAV Texture2D;
        [FieldOffset(0)]
        public D3D12_TEX2D_ARRAY_UAV Texture2DArray;
        [FieldOffset(0)]
        public D3D12_TEX3D_UAV Texture3D;
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_DEPTH_STENCIL_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D12_DSV_DIMENSION ViewDimension;
    public D3D12_DSV_FLAGS Flags;
    private D3D12_DEPTH_STENCIL_VIEW_DESC_UNION _union;
    public ref D3D12_TEX1D_DSV Texture1D => ref ((D3D12_DEPTH_STENCIL_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture1D;
    public ref D3D12_TEX1D_ARRAY_DSV Texture1DArray => ref ((D3D12_DEPTH_STENCIL_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture1DArray;
    public ref D3D12_TEX2D_DSV Texture2D => ref ((D3D12_DEPTH_STENCIL_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2D;
    public ref D3D12_TEX2D_ARRAY_DSV Texture2DArray => ref ((D3D12_DEPTH_STENCIL_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DArray;
    public ref D3D12_TEX2DMS_DSV Texture2DMS => ref ((D3D12_DEPTH_STENCIL_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DMS;
    public ref D3D12_TEX2DMS_ARRAY_DSV Texture2DMSArray => ref ((D3D12_DEPTH_STENCIL_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DMSArray;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_DEPTH_STENCIL_VIEW_DESC_UNION
    {
        [FieldOffset(0)]
        public D3D12_TEX1D_DSV Texture1D;
        [FieldOffset(0)]
        public D3D12_TEX1D_ARRAY_DSV Texture1DArray;
        [FieldOffset(0)]
        public D3D12_TEX2D_DSV Texture2D;
        [FieldOffset(0)]
        public D3D12_TEX2D_ARRAY_DSV Texture2DArray;
        [FieldOffset(0)]
        public D3D12_TEX2DMS_DSV Texture2DMS;
        [FieldOffset(0)]
        public D3D12_TEX2DMS_ARRAY_DSV Texture2DMSArray;
    }
}
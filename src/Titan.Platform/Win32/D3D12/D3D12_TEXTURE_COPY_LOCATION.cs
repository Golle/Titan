using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_TEXTURE_COPY_LOCATION
{
    public ID3D12Resource* pResource;
    public D3D12_TEXTURE_COPY_TYPE Type;
    private D3D12_TEXTURE_COPY_LOCATION_UNION _union;
    public ref D3D12_PLACED_SUBRESOURCE_FOOTPRINT PlacedFootprint => ref ((D3D12_TEXTURE_COPY_LOCATION_UNION*)Unsafe.AsPointer(ref _union))->PlacedFootprint;
    public ref uint SubresourceIndex => ref ((D3D12_TEXTURE_COPY_LOCATION_UNION*)Unsafe.AsPointer(ref _union))->SubresourceIndex;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_TEXTURE_COPY_LOCATION_UNION
    {
        [FieldOffset(0)]
        public D3D12_PLACED_SUBRESOURCE_FOOTPRINT PlacedFootprint;
        [FieldOffset(0)]
        public uint SubresourceIndex;
    }
}
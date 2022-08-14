using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.DXGI;

namespace Titan.Windows.D3D12;

public unsafe struct D3D12_SHADER_RESOURCE_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D12_SRV_DIMENSION ViewDimension;
    public uint Shader4ComponentMapping;
    private D3D12_SHADER_RESOURCE_VIEW_DESC_UNION _union;

    //union
    public ref D3D12_BUFFER_SRV Buffer => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Buffer;
    public ref D3D12_TEX1D_SRV Texture1D => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture1D;
    public ref D3D12_TEX1D_ARRAY_SRV Texture1DArray => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture1DArray;
    public ref D3D12_TEX2D_SRV Texture2D => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2D;
    public ref D3D12_TEX2D_ARRAY_SRV Texture2DArray => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DArray;
    public ref D3D12_TEX2DMS_SRV Texture2DMS => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DMS;
    public ref D3D12_TEX2DMS_ARRAY_SRV Texture2DMSArray => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture2DMSArray;
    public ref D3D12_TEX3D_SRV Texture3D => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->Texture3D;
    public ref D3D12_TEXCUBE_SRV TextureCube => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->TextureCube;
    public ref D3D12_TEXCUBE_ARRAY_SRV TextureCubeArray => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->TextureCubeArray;
    public ref D3D12_RAYTRACING_ACCELERATION_STRUCTURE_SRV RaytracingAccelerationStructure => ref ((D3D12_SHADER_RESOURCE_VIEW_DESC_UNION*)Unsafe.AsPointer(ref _union))->RaytracingAccelerationStructure;
    // end union

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_SHADER_RESOURCE_VIEW_DESC_UNION
    {
        [FieldOffset(0)]
        public D3D12_BUFFER_SRV Buffer;
        [FieldOffset(0)]
        public D3D12_TEX1D_SRV Texture1D;
        [FieldOffset(0)]
        public D3D12_TEX1D_ARRAY_SRV Texture1DArray;
        [FieldOffset(0)]
        public D3D12_TEX2D_SRV Texture2D;
        [FieldOffset(0)]
        public D3D12_TEX2D_ARRAY_SRV Texture2DArray;
        [FieldOffset(0)]
        public D3D12_TEX2DMS_SRV Texture2DMS;
        [FieldOffset(0)]
        public D3D12_TEX2DMS_ARRAY_SRV Texture2DMSArray;
        [FieldOffset(0)]
        public D3D12_TEX3D_SRV Texture3D;
        [FieldOffset(0)]
        public D3D12_TEXCUBE_SRV TextureCube;
        [FieldOffset(0)]
        public D3D12_TEXCUBE_ARRAY_SRV TextureCubeArray;
        [FieldOffset(0)]
        public D3D12_RAYTRACING_ACCELERATION_STRUCTURE_SRV RaytracingAccelerationStructure;
    }
}
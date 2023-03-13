using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_SHADER_RESOURCE_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D12_SRV_DIMENSION ViewDimension;
    public uint Shader4ComponentMapping;
    private D3D12_SHADER_RESOURCE_VIEW_DESC_UNION _union;

    //union
    [UnscopedRef]
    public ref D3D12_BUFFER_SRV Buffer => ref _union.Buffer;
    [UnscopedRef]
    public ref D3D12_TEX1D_SRV Texture1D => ref _union.Texture1D;
    [UnscopedRef]
    public ref D3D12_TEX1D_ARRAY_SRV Texture1DArray => ref _union.Texture1DArray;
    [UnscopedRef]
    public ref D3D12_TEX2D_SRV Texture2D => ref _union.Texture2D;
    [UnscopedRef]
    public ref D3D12_TEX2D_ARRAY_SRV Texture2DArray => ref _union.Texture2DArray;
    [UnscopedRef]
    public ref D3D12_TEX2DMS_SRV Texture2DMS => ref _union.Texture2DMS;
    [UnscopedRef]
    public ref D3D12_TEX2DMS_ARRAY_SRV Texture2DMSArray => ref _union.Texture2DMSArray;
    [UnscopedRef]
    public ref D3D12_TEX3D_SRV Texture3D => ref _union.Texture3D;
    [UnscopedRef]
    public ref D3D12_TEXCUBE_SRV TextureCube => ref _union.TextureCube;
    [UnscopedRef]
    public ref D3D12_TEXCUBE_ARRAY_SRV TextureCubeArray => ref _union.TextureCubeArray;
    [UnscopedRef]
    public ref D3D12_RAYTRACING_ACCELERATION_STRUCTURE_SRV RaytracingAccelerationStructure => ref _union.RaytracingAccelerationStructure;
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

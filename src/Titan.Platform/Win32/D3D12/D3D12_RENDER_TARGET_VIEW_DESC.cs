using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_RENDER_TARGET_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D12_RTV_DIMENSION ViewDimension;
    private D3D12_RENDER_TARGET_VIEW_DESC_UNION UnionMembers;

    [UnscopedRef]
    public ref D3D12_BUFFER_RTV Buffer => ref UnionMembers.Buffer;
    [UnscopedRef]
    public ref D3D12_TEX1D_RTV Texture1D => ref UnionMembers.Texture1D;
    [UnscopedRef]
    public ref D3D12_TEX1D_ARRAY_RTV Texture1DArray => ref UnionMembers.Texture1DArray;
    [UnscopedRef]
    public ref D3D12_TEX2D_RTV Texture2D => ref UnionMembers.Texture2D;
    [UnscopedRef]
    public ref D3D12_TEX2D_ARRAY_RTV Texture2DArray => ref UnionMembers.Texture2DArray;
    [UnscopedRef]
    public ref D3D12_TEX2DMS_RTV Texture2DMS => ref UnionMembers.Texture2DMS;
    [UnscopedRef]
    public ref D3D12_TEX2DMS_ARRAY_RTV Texture2DMSArray => ref UnionMembers.Texture2DMSArray;
    [UnscopedRef]
    public ref D3D12_TEX3D_RTV Texture3D => ref UnionMembers.Texture3D;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_RENDER_TARGET_VIEW_DESC_UNION
    {
        //NOTE(Jens): Add more when we need them. Keep it simple for now
        [FieldOffset(0)]
        public D3D12_BUFFER_RTV Buffer;
        [FieldOffset(0)]
        public D3D12_TEX1D_RTV Texture1D;
        [FieldOffset(0)]
        public D3D12_TEX1D_ARRAY_RTV Texture1DArray;
        [FieldOffset(0)]
        public D3D12_TEX2D_RTV Texture2D;
        [FieldOffset(0)]
        public D3D12_TEX2D_ARRAY_RTV Texture2DArray;
        [FieldOffset(0)]
        public D3D12_TEX2DMS_RTV Texture2DMS;
        [FieldOffset(0)]
        public D3D12_TEX2DMS_ARRAY_RTV Texture2DMSArray;
        [FieldOffset(0)]
        public D3D12_TEX3D_RTV Texture3D;
    }
}

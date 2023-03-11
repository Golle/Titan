using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_RENDER_TARGET_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D12_RTV_DIMENSION ViewDimension;

    private D3D12_RENDER_TARGET_VIEW_DESC_UNION UnionMembers;
    public ref D3D12_TEX2D_RTV Texture2D => ref ((D3D12_RENDER_TARGET_VIEW_DESC_UNION*)Unsafe.AsPointer(ref UnionMembers))->Texture2D;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_RENDER_TARGET_VIEW_DESC_UNION
    {
        //NOTE(Jens): Add more when we need them. Keep it simple for now
        [FieldOffset(0)]
        //D3D12_BUFFER_RTV Buffer;
        //D3D12_TEX1D_RTV Texture1D;
        //D3D12_TEX1D_ARRAY_RTV Texture1DArray;
        public D3D12_TEX2D_RTV Texture2D;
        //D3D12_TEX2D_ARRAY_RTV Texture2DArray;
        //D3D12_TEX2DMS_RTV Texture2DMS;
        //D3D12_TEX2DMS_ARRAY_RTV Texture2DMSArray;
        //D3D12_TEX3D_RTV Texture3D;
    }
}
public unsafe struct D3D12_INPUT_ELEMENT_DESC
{
    public byte* SemanticName;
    public uint SemanticIndex;
    public DXGI_FORMAT Format;
    public uint InputSlot;
    public uint AlignedByteOffset;
    public D3D12_INPUT_CLASSIFICATION InputSlotClass;
    public uint InstanceDataStepRate;
}

public enum D3D12_INPUT_CLASSIFICATION
{
    D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA = 0,
    D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA = 1
}

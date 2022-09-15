using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[StructLayout(LayoutKind.Explicit)]
public struct D3D12_INDIRECT_ARGUMENT_DESC
{
    [FieldOffset(0)] public D3D12_INDIRECT_ARGUMENT_TYPE Type;

    [FieldOffset(sizeof(D3D12_INDIRECT_ARGUMENT_TYPE))]
    public VertexBufferDesc VertexBuffer;

    [FieldOffset(sizeof(D3D12_INDIRECT_ARGUMENT_TYPE))]
    public ConstantDesc Constant;

    [FieldOffset(sizeof(D3D12_INDIRECT_ARGUMENT_TYPE))]
    public ConstantBufferViewDesc ConstantBufferView;

    [FieldOffset(sizeof(D3D12_INDIRECT_ARGUMENT_TYPE))]
    public ShaderResourceViewDesc ShaderResourceView;

    [FieldOffset(sizeof(D3D12_INDIRECT_ARGUMENT_TYPE))]
    public UnorderedAccessViewDesc UnorderedAccessView;

    // Union structs
    public struct VertexBufferDesc
    {
        public uint Slot;
    }

    public struct ConstantDesc
    {
        public uint RootParameterIndex;
        public uint DestOffsetIn32BitValues;
        public uint Num32BitValuesToSet;
    }

    public struct ConstantBufferViewDesc
    {
        public uint RootParameterIndex;
    }

    public struct ShaderResourceViewDesc
    {
        public uint RootParameterIndex;
    }

    public struct UnorderedAccessViewDesc
    {
        public uint RootParameterIndex;
    }
}
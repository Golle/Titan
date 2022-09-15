namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_COMMAND_SIGNATURE_DESC
{
    public uint ByteStride;
    public uint NumArgumentDescs;
    public D3D12_INDIRECT_ARGUMENT_DESC* pArgumentDescs;
    public uint NodeMask;
}
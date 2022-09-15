namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_STREAM_OUTPUT_DESC
{
    public D3D12_SO_DECLARATION_ENTRY* pSODeclaration;
    public uint NumEntries;
    public uint* pBufferStrides;
    public uint NumStrides;
    public uint RasterizedStream;
}
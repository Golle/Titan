namespace Titan.Platform.Win32.D3D12;

public struct D3D12_HEAP_DESC
{
    public ulong SizeInBytes;
    public D3D12_HEAP_PROPERTIES Properties;
    public ulong Alignment;
    public D3D12_HEAP_FLAGS Flags;
}
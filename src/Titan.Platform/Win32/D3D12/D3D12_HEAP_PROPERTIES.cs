namespace Titan.Platform.Win32.D3D12;

public struct D3D12_HEAP_PROPERTIES
{
    public D3D12_HEAP_TYPE Type;
    public D3D12_CPU_PAGE_PROPERTY CPUPageProperty;
    public D3D12_MEMORY_POOL MemoryPoolPreference;
    public uint CreationNodeMask;
    public uint VisibleNodeMask;
}
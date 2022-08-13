using System.Runtime.InteropServices;

namespace Titan.Windows.D3D12;

[StructLayout(LayoutKind.Sequential)]
public struct D3D12_COMMAND_QUEUE_DESC
{
    public D3D12_COMMAND_LIST_TYPE Type;
    /// <summary>
    /// Can use type D3D12_COMMAND_QUEUE_PRIORITY
    /// </summary>
    public int Priority;
    public D3D12_COMMAND_QUEUE_FLAGS Flags;
    public uint NodeMask;
}

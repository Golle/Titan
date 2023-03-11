using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_GPU_DESCRIPTOR_HANDLE
{
    public ulong ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator D3D12_GPU_DESCRIPTOR_HANDLE(ulong ptr) => new() { ptr = ptr };
}

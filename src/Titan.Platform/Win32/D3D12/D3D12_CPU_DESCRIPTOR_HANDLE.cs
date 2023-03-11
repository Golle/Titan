using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_CPU_DESCRIPTOR_HANDLE
{
    public nuint ptr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator D3D12_CPU_DESCRIPTOR_HANDLE(nuint ptr) => new() { ptr = ptr };
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator D3D12_CPU_DESCRIPTOR_HANDLE(void* ptr) => new() { ptr = (nuint)ptr };
}

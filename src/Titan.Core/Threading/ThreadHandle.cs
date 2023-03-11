using System.Runtime.CompilerServices;

namespace Titan.Core.Threading;

public readonly struct ThreadHandle
{
    public readonly nuint Handle;
    public readonly uint Id;
    public ThreadHandle(uint id, nuint handle)
    {
        Id = id;
        Handle = handle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid() => Handle != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nuint(ThreadHandle handle) => handle.Handle;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ulong(ThreadHandle handle) => handle.Handle;

    public static readonly ThreadHandle Invalid = default;
}
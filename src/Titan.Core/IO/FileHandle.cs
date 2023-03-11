using System.Runtime.CompilerServices;

namespace Titan.Core.IO;

public readonly struct FileHandle
{
    public readonly nuint Handle;
    internal FileHandle(nuint handle) => Handle = handle;
    public bool IsValid() => Handle != 0;
    public bool IsInvalid() => Handle == 0;
    public static readonly FileHandle Invalid = new(0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nuint(FileHandle handle) => handle.Handle;
    public override string ToString() => Handle.ToString();
}

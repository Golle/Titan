using System.Runtime.CompilerServices;

namespace Titan.Core.Threading2;

public unsafe struct JobItem
{
    internal delegate*<void*, void> Function;
    internal void* Context;
    internal bool AutoReset;

    public static JobItem Create<T>(ref T context, delegate*<ref T, void> callback, bool autoReset = true) where T : unmanaged =>
        new()
        {
            Context = Unsafe.AsPointer(ref context),
            AutoReset = autoReset,
            Function = (delegate*<void*, void>)callback
        };
}

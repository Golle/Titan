using System.Runtime.CompilerServices;

namespace Titan.Core.Threading2;

public unsafe struct JobItem
{
    internal delegate*<void*, void> Function;
    internal void* Context;
    internal bool AutoReset;

    public static JobItem Create<T>(ref T context, delegate*<ref T, void> callback, bool autoReset = true) where T : unmanaged
        => Create(Unsafe.AsPointer(ref context), (delegate*<void*, void>)callback, autoReset);

    public static JobItem Create(void* context, delegate*<void*, void> callback, bool autoReset = true)
        => new()
        {
            Context = context,
            AutoReset = autoReset,
            Function = callback
        };
}
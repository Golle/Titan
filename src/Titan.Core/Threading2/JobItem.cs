using Titan.Core.Memory;

namespace Titan.Core.Threading2;

public unsafe struct JobItem
{
    internal delegate*<void*, void> Function;
    internal void* Context;
    internal bool AutoReset;
    internal bool IsReady;

    public static JobItem Create<T>(ref T context, delegate*<ref T, void> callback, bool isReady = true, bool autoReset = true) where T : unmanaged
        => Create(MemoryUtils.AsPointer(ref context), (delegate*<void*, void>)callback, isReady, autoReset);

    public static JobItem Create(void* context, delegate*<void*, void> callback, bool isReady = true, bool autoReset = true)
        => new()
        {
            Context = context,
            AutoReset = autoReset,
            Function = callback,
            IsReady = isReady
        };
}

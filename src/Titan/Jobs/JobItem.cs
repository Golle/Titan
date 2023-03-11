using Titan.Core.Memory;

namespace Titan.Jobs;

public unsafe struct JobItem
{
    internal delegate*<void*, void> Function;
    internal void* Context;
    internal bool AutoReset;
    internal bool IsReady;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context">The reference must be a pinned unmanaged struct that has a longer lifetime than the current scope.</param>
    /// <param name="callback"></param>
    /// <param name="isReady"></param>
    /// <param name="autoReset"></param>
    /// <returns></returns>
    public static JobItem Create<T>(ref T context, delegate*<ref T, void> callback, bool isReady = true, bool autoReset = true) where T : unmanaged
        => Create(MemoryUtils.AsPointer(context), (delegate*<void*, void>)callback, isReady, autoReset);

    public static JobItem Create(void* context, delegate*<void*, void> callback, bool isReady = true, bool autoReset = true)
        => new()
        {
            Context = context,
            AutoReset = autoReset,
            Function = callback,
            IsReady = isReady
        };
}

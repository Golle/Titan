using System.Diagnostics;
using Titan.Core.Logging;

namespace Titan.Core;

internal static class GCHandleCounter
{
    private static volatile int _handleCounter;

    //NOTE(Jens): move this to something that can track any type of information we want    
    [Conditional("DEBUG")]
    public static void GCHandleAlloced() => Interlocked.Increment(ref _handleCounter);

    [Conditional("DEBUG")]
    public static void GCHandleFreed() => Interlocked.Decrement(ref _handleCounter);

    [Conditional("DEBUG")]
    public static void Assert()
    {
        if (_handleCounter != 0)
        {
            Logger.Warning($"There are currently {_handleCounter} handles that has not been released. (Not sure how these should be tracked and released)", typeof(GCHandleCounter));
        }
        else
        {
            Logger.Trace("No leaked handles.", typeof(GCHandleCounter));
        }
    }
}

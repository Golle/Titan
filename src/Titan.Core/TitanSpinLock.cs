using System.Runtime.CompilerServices;

namespace Titan.Core;

public struct TitanSpinLock
{
    private volatile int _lock;
    public void WaitForAccess()
    {
        var sw = new SpinWait();
        while (true)
        {
            if (Interlocked.CompareExchange(ref _lock, 1, 0) == 0)
            {
                break;
            }
            sw.SpinOnce();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Release() => _lock = 0;
}
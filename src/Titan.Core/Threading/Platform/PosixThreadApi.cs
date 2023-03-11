using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Core.Threading.Platform;

internal class PosixThreadApi : ManagedThreadApi
{
    //NOTE(Jens): Currently using the ManagerThreadAPI. Should implement a proper pthread API later.
}

internal class ManagedThreadApi : IThreadApi
{
    public static unsafe ThreadHandle CreateThread(delegate* unmanaged<void*, int> callback, void* parameter, bool startImmediately = false)
    {
        var thread = new Thread(() => callback(parameter));
        if (startImmediately)
        {
            thread.Start();
        }
        var handle = GCHandle.Alloc(thread);
        return new ThreadHandle((uint)thread.ManagedThreadId, (nuint)GCHandle.ToIntPtr(handle));
    }

    public static bool Start(in ThreadHandle handle)
    {
        var thread = ToThread(handle);
        thread.Start();
        return true;
    }

    public static bool Join(in ThreadHandle handle)
    {
        var thread = ToThread(handle);
        thread.Join();
        return true;
    }

    public static void Destroy(ref ThreadHandle handle)
    {
        var gcHandle = GCHandle.FromIntPtr((nint)handle.Handle);
        Debug.Assert(gcHandle.IsAllocated);
        gcHandle.Target = null;
        gcHandle.Free();
    }

    public static void Sleep(uint milliseconds) 
        => Thread.Sleep((int)milliseconds);

    public static uint GetCurrentThreadId() 
        => (uint)Thread.CurrentThread.ManagedThreadId;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Thread ToThread(in ThreadHandle handle)
    {
        var gcHandle = GCHandle.FromIntPtr((nint)handle.Handle);
        Debug.Assert(gcHandle.IsAllocated);
        Debug.Assert(gcHandle.Target != null);
        return (Thread)gcHandle.Target;
    }
}

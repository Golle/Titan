// comment out this row to disable tracing of win32 thread api calls.
//#define WIN32_THREAD_TRACE

using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Platform.Win32;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Core.Threading.Platform;

public unsafe class Win32ThreadApi : IThreadApi
{
    [Conditional("WIN32_THREAD_TRACE")]
    private static void Trace(string message) => Logger.Trace<Win32ThreadApi>(message);

    private const uint CREATE_SUSPENDED = 0x00000004;
    public static ThreadHandle CreateThread(delegate* unmanaged<void*, int> callback, void* parameter, bool startImmediately = false)
    {
        uint threadId = 0;
        var handle = Kernel32.CreateThread(null, 0, callback, parameter, startImmediately ? 0 : CREATE_SUSPENDED, &threadId);
        if (!handle.IsValid())
        {
            Logger.Error<Win32ThreadApi>("Failed to create a thread.");
            return ThreadHandle.Invalid;
        }
        Trace($"Created thread with ID {threadId} and handle {handle} from Thread with ID {GetCurrentThreadId()}");
        return new ThreadHandle(threadId, handle);
    }

    public static bool Start(in ThreadHandle handle)
    {
        Trace($"Starting thread with ID {handle.Id} and handle {handle.Handle} from Thread with ID {GetCurrentThreadId()}");
        Debug.Assert(handle.IsValid());
        var result = Kernel32.ResumeThread(handle.Handle);
        if (result == uint.MaxValue)
        {
            Logger.Error<Win32ThreadApi>($"Failed to resume thread with handle {handle.Handle} and ID {handle.Id}");
            return false;
        }
        return true;
    }

    public static bool Join(in ThreadHandle handle)
    {
        Trace($"Joining thread with ID {handle.Id} and handle {handle.Handle} onto current Thread with ID {GetCurrentThreadId()}");
        //NOTE(Jens): add a timeout
        var result = Kernel32.WaitForSingleObject(handle.Handle, INFINITE);
        if (result != 0)
        {
            Logger.Warning<Win32ThreadApi>($"{nameof(Kernel32.WaitForSingleObject)} return unexpected code: 0x{result:x8}");
            return false;
        }
        return true;
    }

    public static void Destroy(ref ThreadHandle handle)
    {
        Trace($"Destroying thread with ID {handle.Id} and handle {handle.Handle} from Thread with ID {GetCurrentThreadId()}");
        if (handle.IsValid())
        {
            Kernel32.CloseHandle(handle.Handle);
            handle = default;
        }
    }

    public static void Sleep(uint milliseconds)
    {
        Trace($"Sleep thread {GetCurrentThreadId()} for {milliseconds} ms");
        Kernel32.Sleep(milliseconds);
    }

    public static uint GetCurrentThreadId()
        => Kernel32.GetCurrentThreadId();
}

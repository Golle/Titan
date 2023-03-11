namespace Titan.Core.Threading;

public unsafe interface IThreadApi
{
    static abstract ThreadHandle CreateThread(delegate* unmanaged<void*, int> callback, void* parameter, bool startImmediately = false);
    static abstract bool Start(in ThreadHandle handle);
    static abstract bool Join(in ThreadHandle handle);
    static abstract void Destroy(ref ThreadHandle handle);
    static abstract void Sleep(uint milliseconds);
    static abstract uint GetCurrentThreadId();
}

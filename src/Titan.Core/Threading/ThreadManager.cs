namespace Titan.Core.Threading;

internal unsafe class ThreadManager<T> : IThreadManager where T : IThreadApi
{
    public ThreadHandle CreateThread(in CreateThreadArgs args) => T.CreateThread(args.Callback, args.Parameter, args.StartImmediately);
    public bool Start(in ThreadHandle handle) => T.Start(handle);
    public bool Join(in ThreadHandle handle) => T.Join(handle);
    public void Destroy(ref ThreadHandle handle) => T.Destroy(ref handle);
    public void Sleep(uint milliseconds) => T.Sleep(milliseconds);
    public uint GetCurrentThreadId() => T.GetCurrentThreadId();
}

namespace Titan.Core.Threading;

public interface IThreadManager
{
    ThreadHandle CreateThread(in CreateThreadArgs args);
    bool Start(in ThreadHandle handle);
    bool Join(in ThreadHandle handle);
    void Destroy(ref ThreadHandle handle);
    void Sleep(uint milliseconds);
    uint GetCurrentThreadId();
}

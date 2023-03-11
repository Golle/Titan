using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Jobs;

[DebuggerDisplay("JobHandle: {Handle}")]
public readonly struct JobHandle
{
    public readonly int Handle;
    internal JobHandle(int handle)
    {
        Handle = handle;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid() => Handle != 0;
    public static readonly JobHandle Invalid = default;

    public override string ToString()
    {
        return Handle.ToString();
    }
}


public interface IJobApi
{
    JobHandle Enqueue(in JobItem jobItem);
    bool IsCompleted(in JobHandle handle);
    void Reset(ref JobHandle handle);
}

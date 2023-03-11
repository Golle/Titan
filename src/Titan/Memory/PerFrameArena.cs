using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;

namespace Titan.Memory;

internal unsafe struct PerFrameArena : IResource
{
    private readonly byte* _mem;
    private volatile uint _offset;
    private readonly uint _size;
    public PerFrameArena(void* mem, uint size)
    {
        _size = size;
        _mem = (byte*)mem;
        _offset = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(uint size)
    {
        Debug.Assert(_offset + size < _size, "All temp memory used, can't allocate more. Update the settings to support bigger buffers.");
        //NOTE(Jens): Maybe we should implement an aligned version?
        var offset = Interlocked.Add(ref _offset, size) - size;
        return _mem + offset;
    }
    public void Reset()
    {
        _offset = 0;
    }

    public void* GetPointer() => _mem;
}

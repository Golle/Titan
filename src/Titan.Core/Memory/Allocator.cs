using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.Core.Memory;

public unsafe struct Allocator
{
    // NOTE(Jens): align all allocations to 8 bytes. (We should measure this at some point)
    private const int Alignment = 8;

    private readonly byte* _memory;
    private readonly uint _size;
    private volatile int _next;
    
    public uint Size => _size;
    public uint Used => (uint)_next;
    public Allocator(void* memory, uint size)
    {
        _memory = (byte*)memory;
        _size = size;
    }

    public T* GetPointer<T>(bool initialize) where T : unmanaged
        => (T*)GetOffset((uint)sizeof(T), initialize);

    public void* GetPointer(uint size, bool zeroMemory)
        => GetOffset(size, zeroMemory);

    private void* GetOffset(uint size, bool zeroMemory)
    {
        size = (uint)((size + Alignment - 1) & -Alignment);

        var blockEnd = Interlocked.Add(ref _next, (int)size);
        Debug.Assert(blockEnd <= _size, "The requested memory block is out of range.");
        var location = _memory + blockEnd - size;
        if (zeroMemory)
        {
            Unsafe.InitBlock(location, 0, size);
        }
        return location;
    }
    public void Reset(bool initialize = false)
    {
        _next = 0;
        if (initialize)
        {
            Unsafe.InitBlock(_memory, 0, _size);
        }
    }
}

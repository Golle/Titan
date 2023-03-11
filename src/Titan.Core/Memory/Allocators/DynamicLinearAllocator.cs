using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.Memory.Allocators;

public unsafe class DynamicLinearAllocator : ILinearAllocator
{
    //NOTE(Jens): This is not thread safe. If we want to use a single allocator from multiple threads we need to fix that. probably better to create a new type 
    private VirtualMemoryBlock _memory;
    private uint _offset;
    internal static bool TryCreate(MemoryManager memoryManager, uint size, out ILinearAllocator allocator)
    {
        Unsafe.SkipInit(out allocator);
        if (!memoryManager.TryReserveVirtualMemoryBlock(size, out var block))
        {
            Logger.Error<DynamicLinearAllocator>($"Failed to reserve a virtual memory block of {size} bytes.");
            return false;
        }
        allocator = new DynamicLinearAllocator
        {
            _memory = block,
            _offset = 0
        };

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(int size, bool initialize = false)
    {
        Debug.Assert(size >= 0);
        return Alloc((uint)size, initialize);
    }

    public void* Alloc(uint size, bool initialize = false)
    {
        var alignedSize = MemoryUtils.AlignToUpper(size);
        while (_offset + alignedSize > _memory.Size)
        {
            Expand(alignedSize);
        }

        var mem = (byte*)_memory.Mem + _offset;
        _offset += alignedSize;

        if (initialize)
        {
            MemoryUtils.Init(mem, size);
        }
        return mem;
    }

    [MethodImpl(MethodImplOptions.NoInlining)] // no inlining might not make a difference, but in theory it wont insert a lot of instructions in the alloc code that will only be called very few times.
    private void Expand(uint size)
    {
        var currentSize = _memory.Size;
        if (currentSize == 0)
        {
            // first call to expand, just use whatever size was requested.
            _memory.Resize(size);
        }
        else
        {
            var newSize = (nuint)Math.Max(_memory.Size * 2u, size); // increase with double size or the size of the block requested
            _memory.Resize(newSize);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Alloc<T>(uint count = 1, bool initialize = false) where T : unmanaged => (T*)Alloc((uint)(sizeof(T) * count), initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray AllocArray(uint count, uint stride, bool initialize = false) => new(Alloc(count * stride, initialize), count, stride);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray<T> AllocArray<T>(int count, bool initialize = false) where T : unmanaged
    {
        Debug.Assert(count > 0);
        return AllocArray<T>((uint)count, initialize);
    }

    public TitanArray<T> AllocArray<T>(uint count, bool initialize = false) where T : unmanaged => new(Alloc<T>(count, initialize), count);
    public void Reset() => _offset = 0;
    public void Destroy()
    {
        if (_memory.Mem != null)
        {
            _memory.Release();
            _memory = default;
        }
    }
    public uint GetBytesAllocated() => _offset;
}

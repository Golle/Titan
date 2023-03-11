using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory.Allocators;

namespace Titan.Core.Memory;

public record struct MemoryConfiguration(nuint MaxVirtualMemory, nuint GeneralPurposeMemory);
public enum AllocatorStrategy
{
    /// <summary>
    /// A Permanent strategy will allocate a new block in the Virtual Memory, it can be released but the address space will not be returned.
    /// </summary>
    Permanent,
    /// <summary>
    /// A temporary stratgey will allocate a block using the General Purpose allocator, can be freed and the memory will be available for use again.
    /// </summary>
    Temporary
}

internal unsafe class MemoryManager : IMemoryManager
{
    private VirtualMemory _virtualMemory;
    private GeneralAllocator _generalAllocator;
    public bool Init(MemoryConfiguration config)
    {
        Logger.Trace<MemoryManager>($"Trying to reserve {config.MaxVirtualMemory} bytes of memory and create a {nameof(GeneralAllocator)} with {config.GeneralPurposeMemory} bytes.");
        var allocator = PlatformAllocatorHelper.GetPlatformAllocator();
        Debug.Assert(allocator != null);
        if (!VirtualMemory.TryCreate(allocator, config.MaxVirtualMemory, out var memory))
        {
            Logger.Error<MemoryManager>("Failed to create the virtual memory block");
            return false;
        }

        if (!memory.TryReserveBlock(config.GeneralPurposeMemory, out var block))
        {
            Logger.Error<MemoryManager>($"Failed to reserve a block of {config.GeneralPurposeMemory} bytes for a {nameof(GeneralAllocator)}");
            memory.Release();
            return false;
        }

        _generalAllocator = new GeneralAllocator(block);
        _virtualMemory = memory;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(int size, bool initialize = false)
    {
        Debug.Assert(size >= 0);
        return Alloc((uint)size, initialize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Alloc(uint size, bool initialize = false)
        => _generalAllocator.Allocate(size, initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Alloc<T>(uint count, bool initialize) where T : unmanaged
        => _generalAllocator.Allocate<T>(count, initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanBuffer AllocBuffer(uint size, bool initialize = false)
        => _generalAllocator.AllocateBuffer(size, initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray<T> AllocArray<T>(uint count, bool initialize = false) where T : unmanaged
        => _generalAllocator.AllocateArray<T>(count, initialize);

    public TitanQueue<T> AllocQueue<T>(uint count) where T : unmanaged 
        => _generalAllocator.AllocateQueue<T>(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAllocArray<T>(out TitanArray<T> array, uint count, bool initialize = false) where T : unmanaged
        => _generalAllocator.TryAllocateArray(out array, count, initialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAllocQueue<T>(out TitanQueue<T> queue, uint count) where T : unmanaged 
        => _generalAllocator.TryAllocateQueue(out queue, count);

    //NOTE(Jens): Add a check if it's a pointer within the memory blocks
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Free(void* ptr)
        => _generalAllocator.Free(ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Free(ref TitanBuffer buffer)
        => _generalAllocator.Free(ref buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Free<T>(ref TitanArray<T> array) where T : unmanaged
        => _generalAllocator.Free(ref array);

    public bool TryCreatePoolAllocator<T>(AllocatorStrategy allocatorStrategy, uint maxCount, out IPoolAllocator<T> allocator) where T : unmanaged =>
        allocatorStrategy switch
        {
            AllocatorStrategy.Permanent => throw new NotSupportedException($"Allocation Strategy {allocatorStrategy} is not supported yet."),
            AllocatorStrategy.Temporary or _ => FixedSizePoolAllocator<T>.TryCreate(this, maxCount, out allocator)
        };

    public bool TryCreateLinearAllocator(AllocatorStrategy allocatorStrategy, uint size, out ILinearAllocator allocator) =>
        allocatorStrategy switch
        {
            AllocatorStrategy.Permanent => DynamicLinearAllocator.TryCreate(this, size, out allocator),
            AllocatorStrategy.Temporary or _ => throw new NotSupportedException($"Allocation strategy {allocatorStrategy} is not supported yet for {nameof(ILinearAllocator)}")
        };

    public bool TryCreateGeneralAllocator(uint minSize, out IGeneralAllocator allocator)
    {
        Unsafe.SkipInit(out allocator);
        if (!TryReserveVirtualMemoryBlock(minSize, out var block))
        {
            Logger.Error<MemoryManager>($"Failed to reserve {minSize} bytes of memory for a {nameof(GeneralAllocator)}.");
            return false;
        }
        allocator = new GeneralAllocator(block);
        return true;
    }

    internal bool TryReserveVirtualMemoryBlock(nuint size, out VirtualMemoryBlock block)
    {
        var result = _virtualMemory.TryReserveBlock(size, out block);
        if (!result)
        {
            Logger.Error<MemoryManager>($"Failed to reserve {size} bytes.");
        }
        return result;
    }

    public void Shutdown()
    {
        _generalAllocator.Release();
        _virtualMemory.Release(); ;
    }
}

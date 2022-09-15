using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Memory.Allocators;

namespace Titan.Memory;

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

public record struct LinearAllocatorArgs(uint Size, AllocatorStrategy Strategy)
{
    public static LinearAllocatorArgs Temporary(uint size) => new(size, AllocatorStrategy.Temporary);
    public static LinearAllocatorArgs Permanent(uint size) => new(size, AllocatorStrategy.Permanent);
}

public record struct PoolAllocatorArgs(uint Count, AllocatorStrategy Strategy)
{
    public static PoolAllocatorArgs Temporary(uint count) => new(count, AllocatorStrategy.Temporary);
    public static PoolAllocatorArgs Permanent(uint count) => new(count, AllocatorStrategy.Permanent);
}

public unsafe struct MemoryManager
{
    private InternalState* _state;
    private MemoryManager(InternalState* state)
    {
        _state = state;
    }
    public static bool Create(in MemoryConfiguration config, out MemoryManager manager)
    {
        var platformAllocator = PlatformAllocatorHelper.GetPlatformAllocator();
        manager = default;
        if (!VirtualMemory.Create(platformAllocator, config.MaxVirtualMemory, out var virtualMemory))
        {
            Logger.Error<MemoryManager>($"Failed to create {nameof(VirtualMemory)} with {config.MaxVirtualMemory} bytes.");
            return false;
        }

        if (!virtualMemory.TryReserveBlock(config.GeneralPurposeMemory, out var block))
        {
            Logger.Error<MemoryManager>($"Failed to reserve a block of {config.GeneralPurposeMemory} bytes for a {nameof(GeneralAllocator)}");
            virtualMemory.Release();
            return false;
        }

        var generalAllocator = new GeneralAllocator(block);
        var state = generalAllocator.Allocate<InternalState>();
        state->GeneralAllocator = generalAllocator;
        state->VirtualMemory = virtualMemory;

        manager = new MemoryManager(state);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void* Alloc(uint size, bool initialize = true)
    {
        Debug.Assert(_state != null);
        var mem = _state->GeneralAllocator.Allocate(size);
        if (initialize)
        {
            MemoryUtils.Init(mem, size);
        }
        return mem;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T* Alloc<T>(bool initialize = true) where T : unmanaged
    {
        Debug.Assert(_state != null);
        var mem = _state->GeneralAllocator.Allocate<T>();
        if (initialize)
        {
            MemoryUtils.Init(mem, sizeof(T));
        }
        return mem;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly TitanArray<T> AllocArray<T>(uint count, bool initialize = true) where T : unmanaged
    {
        Debug.Assert(_state != null);
        var mem = _state->GeneralAllocator.Allocate<T>(count);
        if (initialize)
        {
            MemoryUtils.Init(mem, sizeof(T) * count);
        }
        return new TitanArray<T>(mem, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Free<T>(in TitanArray<T> array) where T : unmanaged
    {
        Debug.Assert(_state != null);
        _state->GeneralAllocator.Free(array.GetPointer());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Free(void* ptr)
    {
        Debug.Assert(_state != null);
        _state->GeneralAllocator.Free(ptr);
    }

    /// <summary>
    /// Creates a LinearAllocator
    /// Permanent strategey will reserve a VirtualMemory block and only allocate pages when needed.
    /// Temporary will allocate all memory att creation from the GeneralAllocator
    /// </summary>
    /// <param name="args">LinearAllocator configuration</param>
    /// <param name="allocator">The linear allocator</param>
    /// <returns>true on success</returns>
    public readonly bool CreateLinearAllocator(in LinearAllocatorArgs args, out LinearAllocator allocator)
    {
        var memoryManagerPtr = MemoryUtils.AsPointer(this);
        return args.Strategy switch
        {
            AllocatorStrategy.Permanent => LinearAllocator.Create<DynamicLinearAllocator>(memoryManagerPtr, args.Size, out allocator),
            AllocatorStrategy.Temporary or _ => LinearAllocator.Create<FixedSizeLinearAllocator>(memoryManagerPtr, args.Size, out allocator)
        };
    }

    /// <summary>
    /// Creates a PoolAllocator&lt;T&gt;
    /// Permanent strategey will reserve a VirtualMemory block and only allocate pages when needed.
    /// Temporary will allocate all memory att creation from the GeneralAllocator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args">PoolAllocator configuration</param>
    /// <param name="allocator">The pool allocator</param>
    /// <returns>true on success</returns>
    public readonly bool CreatePoolAllocator<T>(in PoolAllocatorArgs args, out PoolAllocator<T> allocator) where T : unmanaged
    {
        var memoryManagerPtr = MemoryUtils.AsPointer(this);
        return args.Strategy switch
        {
            AllocatorStrategy.Permanent => PoolAllocator<T>.Create<DynamicPoolAllocator<T>>(memoryManagerPtr, args.Count, out allocator),
            AllocatorStrategy.Temporary or _ => PoolAllocator<T>.Create<FixedSizePoolAllocator<T>>(memoryManagerPtr, args.Count, out allocator)
        };
    }

    public readonly bool CreateGeneralPurposeAllocator(nuint reserveSize, out GeneralAllocator allocator)
    {
        allocator = default;
        if (!_state->VirtualMemory.TryReserveBlock(reserveSize, out var block))
        {
            Logger.Error<MemoryManager>($"Failed to reserve a block of {reserveSize} bytes for a {nameof(GeneralAllocator)}");
            return false;
        }
        allocator = new GeneralAllocator(block);
        return true;
    }

    internal bool TryReserveVirtualMemoryBlock(nuint size, out VirtualMemoryBlock block)
    {
        Debug.Assert(_state != null);

        var result = _state->VirtualMemory.TryReserveBlock(size, out block);
        if (!result)
        {
            Logger.Error<MemoryManager>($"Failed to reserve {size} bytes.");
        }
        return result;
    }

    public void Shutdown()
    {
        if (_state != null)
        {
            //NOTE(Jens): we copy the state variable onto the stack to prevent memory corruption. (when Release is called the memory pointers are set to null, if the allocator is in the same memory we'll get a memory access violation)
            var stateCopy = *_state;
            stateCopy.GeneralAllocator.Free(_state);

            // release the allocators
            stateCopy.GeneralAllocator.Release();
            stateCopy.VirtualMemory.Release();

            //NOTE(Jens): releasing the VirtualMemory will invalidate this entire struct, accessing any private members after that call will result in AccessViolation
        }
    }

    private struct InternalState
    {
        public VirtualMemory VirtualMemory;
        public GeneralAllocator GeneralAllocator;
    }
}

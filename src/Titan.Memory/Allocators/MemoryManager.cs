using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Memory.Allocators;

public record struct MemoryConfiguration(nuint MaxGeneralMemory);

public unsafe struct MemoryManager
{
    private PlatformAllocator* _platformAllocator;
    private InternalState* _state;

    public bool Initialize(PlatformAllocator* platformAllocator, in MemoryConfiguration config)
    {
        _platformAllocator = platformAllocator;
        if (!CreateGeneralPurposeAllocator(config.MaxGeneralMemory, 0, out var generalAllocator))
        {
            Console.WriteLine("Initialzation failed.");
            return false;
        }
        _state = generalAllocator.Allocate<InternalState>();
        _state->GeneralAllocator = generalAllocator;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* Allocate(uint size)
    {
        Debug.Assert(_state != null);
        return _state->GeneralAllocator.Allocate(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Allocate<T>() where T : unmanaged
    {
        Debug.Assert(_state != null);
        return _state->GeneralAllocator.Allocate<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Free(void* ptr)
    {
        Debug.Assert(_state != null);
        _state->GeneralAllocator.Free(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref GeneralAllocator GetGeneralPurposeAllocator()
    {
        Debug.Assert(_state != null);
        return ref _state->GeneralAllocator;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneralAllocator* GetGeneralPurposeAllocatorPointer()
    {
        Debug.Assert(_state != null);
        return &_state->GeneralAllocator;
    }

    public bool CreateDynamicLinearAllocator(uint maxSize, out LinearDynamicSizeAllocator allocator)
    {
        Debug.Assert(_platformAllocator != null);
        if (!LinearDynamicSizeAllocator.Create(_platformAllocator, maxSize, out allocator))
        {
            Console.WriteLine("Failed to create the allocator.");
            return false;
        }
        return true;
    }

    public bool CreateFixedSizeLinearAllocator(uint size, out LinearFixedSizeAllocator allocator)
    {
        Debug.Assert(_state != null);
        if (!LinearFixedSizeAllocator.Create(&_state->GeneralAllocator, size, out allocator))
        {
            Console.WriteLine($"Failed to create the {nameof(LinearFixedSizeAllocator)}");
            return false;
        }
        return true;
    }

    public bool CreateFixedSizePoolAllocator<T>(uint count, bool alignPerObject, out FixedSizePoolAllocator<T> allocator) where T : unmanaged
    {
        if (!FixedSizePoolAllocator<T>.Create(&_state->GeneralAllocator, count, alignPerObject, out allocator))
        {
            Console.WriteLine($"Failed to create the {nameof(FixedSizePoolAllocator<T>)} with type {typeof(T).Name}");
            return false;
        }
        return true;
    }

    public bool CreateDynamicPoolAllocator<T>(uint count, bool alignPerObject, out DynamicPoolAllocator<T> allocator) where T : unmanaged
    {
        if (!DynamicPoolAllocator<T>.Create(_platformAllocator, count, alignPerObject, out allocator))
        {
            Console.WriteLine($"Failed to create the {nameof(DynamicPoolAllocator<T>)} with type {typeof(T).Name}");
            return false;
        }
        return true;
    }

    public bool CreateGeneralPurposeAllocator(nuint reserveSize, uint initialSize, out GeneralAllocator allocator)
    {
        if (!GeneralAllocator.Create(_platformAllocator, reserveSize, initialSize, out allocator))
        {
            Console.WriteLine($"Failed to create the {nameof(GeneralAllocator)}");
            return false;
        }
        return true;
    }

    public void Shutdown()
    {
        if (_state != null)
        {
            //NOTE(Jens): we copy the state variable onto the stack to prevent memory corruption. (when Release is called the memory pointers are set to null, if the allocator is in the same memory we'll get a memory access violation)
            var stateCopy = *_state;
            _state->GeneralAllocator.Free(_state);

            // release the allocators
            stateCopy.GeneralAllocator.Release();

            _state = null;
        }
    }

    private struct InternalState
    {
        public GeneralAllocator GeneralAllocator;
    }
}

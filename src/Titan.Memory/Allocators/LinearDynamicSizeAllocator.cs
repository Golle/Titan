namespace Titan.Memory.Allocators;

public unsafe struct LinearDynamicSizeAllocator
{
    private VirtualMemory _memory;
    private uint _offset;
    public static bool Create(PlatformAllocator* platformAllocator, uint maxSize, out LinearDynamicSizeAllocator allocator)
    {
        allocator = default;
        if (!VirtualMemory.Create(platformAllocator, maxSize, out var virtualMemory))
        {
            return false;
        }

        allocator = new()
        {
            _memory = virtualMemory
        };
        return true;
    }

    public T* Allocate<T>() where T : unmanaged => Allocate<T>(1);

    public T* Allocate<T>(uint count) where T : unmanaged
    {
        var size = (uint)(sizeof(T) * count);
        if (_offset + size > _memory.Size)
        {
            Expand(size);
        }
        var mem = (byte*)_memory.Mem + _offset;
        _offset += size;

        return (T*)mem;
    }


    public void Reset()
    {
        _offset = 0;
    }

    public void Release()
    {
        _memory.Release();
        _offset = 0;
        _memory = default;
    }

    private void Expand(nuint size)
    {
        var currentSize = _memory.Size;
        if (currentSize == 0)
        {
            _memory.Resize(size);
        }
        else
        {
            //NOTE(Jens): double the current size or just increase with whatever size was requested?
            //_memory.Resize(_memory.Size + size); // Increase with the size requested (will be page aligned)
            var newSize = Math.Max(_memory.Size * 2, size); // increase with double size.
            _memory.Resize((nuint)newSize);
        }
    }
}

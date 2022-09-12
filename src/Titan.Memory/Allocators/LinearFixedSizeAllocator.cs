using System.Diagnostics;
using Titan.Core.Memory;

namespace Titan.Memory.Allocators;

public unsafe struct LinearFixedSizeAllocator
{
    private readonly GeneralAllocator* _allocator;
    private byte* _mem;
    private readonly uint _size;
    private int _offset; //NOTE(Jens): only supports up to 2gb

    public LinearFixedSizeAllocator(GeneralAllocator* allocator, void* mem, uint size)
    {
        _mem = (byte*)mem;
        _allocator = allocator;
        _size = size;
        _offset = 0;
    }

    public static bool Create(GeneralAllocator* allocator, uint size, out LinearFixedSizeAllocator linearAllocator)
    {
        Debug.Assert(size < int.MaxValue, $"Max size is {int.MaxValue} bytes");
        var mem = allocator->Allocate(size);
        Debug.Assert(mem != null);
        linearAllocator = new LinearFixedSizeAllocator(allocator, mem, size);
        return true;

    }
    public T* Allocate<T>() where T : unmanaged
        => Allocate<T>(1);

    public T* Allocate<T>(uint count) where T : unmanaged
    {
        Console.WriteLine($"{nameof(Allocate)}. Offset: {_offset}. Size: {sizeof(T)}");
        var allocationSize = sizeof(T) * count;
        Debug.Assert(_mem != null);
        Debug.Assert(count > 0);
        Debug.Assert(_offset + allocationSize < _size);

        var mem = _mem + _offset;
        _offset += (int)allocationSize;
        return (T*)mem;
    }

    public T* AllocateAligned<T>() where T : unmanaged
        => AllocateAligned<T>(1);
    public T* AllocateAligned<T>(uint count) where T : unmanaged
    {
        var alignedSize = MemoryUtils.AlignToUpper((uint)sizeof(T));
        var alignedOffset = MemoryUtils.AlignToUpper((uint)_offset);
        Console.WriteLine($"{nameof(AllocateAligned)}. Offset: {_offset} AlignedOffset: {alignedOffset}. Size: {sizeof(T)} AlignedSize: {alignedSize}");

        var allocationSize = alignedSize * count;
        Debug.Assert(_offset + allocationSize < _size);

        _offset = (int)(alignedOffset + allocationSize);
        return (T*)_mem + alignedOffset;
    }


    public void Reset()
    {
        _offset = 0;
    }

    public void Release()
    {
        if (_mem != null)
        {
            _allocator->Free(_mem);
            _mem = null;
            _offset = 0;
        }
    }


    public void DebugPrint()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Memory: 0x{(nuint)_mem,-16} | Offset: {_offset} | MaxSize: {_size}");
        Console.ResetColor();
    }
}

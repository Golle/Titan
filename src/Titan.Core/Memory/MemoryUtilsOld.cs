using System;
using System.Runtime.InteropServices;

namespace Titan.Core.Memory;

public static unsafe class MemoryUtilsOld
{
    public static MemoryChunk<T> AllocateBlock<T>(uint count, bool zeroMemory = false) where T : unmanaged
    {
        static void OutOfMemoryException() => throw new OutOfMemoryException("Failed to allocate unmanaged memory.");
        var size = (uint)sizeof(T) * count;

        var ptr = zeroMemory ? NativeMemory.AllocZeroed(count, (nuint)sizeof(T)) : NativeMemory.Alloc(count, (nuint)sizeof(T));
        if (ptr == null)
        {
            OutOfMemoryException();
        }
        return new MemoryChunk<T>((T*)ptr, size, count);
    }

    public static MemoryChunk AllocateBlock(uint size, bool zeroMemory = false)
    {
        static void OutOfMemoryException() => throw new OutOfMemoryException("Failed to allocate unmanaged memory.");
        var ptr = zeroMemory ? NativeMemory.AllocZeroed((nuint)size) : NativeMemory.Alloc((nuint)size);
        if (ptr == null)
        {
            OutOfMemoryException();
        }
        return new MemoryChunk(ptr, size);
    }

        
    public static MemoryChunk<T> ReAllocate<T>(ref MemoryChunk<T> memoryChunk, uint count) where T : unmanaged
    {
        static void OutOfMemoryException() => throw new OutOfMemoryException("Failed to allocate unmanaged memory.");
        static void NullPointerException() => throw new InvalidOperationException("Trying to re-allocate a null pointer");
        if (memoryChunk.AsPointer() == null)
        {
            NullPointerException();
        }

        var size = (uint)(sizeof(T) * count);
        var newPointer = NativeMemory.Realloc(memoryChunk.AsPointer(), size);
        if (newPointer == null)
        {
            OutOfMemoryException();
        }
        return memoryChunk = new MemoryChunk<T>((T*)newPointer, size, count);
    }
}
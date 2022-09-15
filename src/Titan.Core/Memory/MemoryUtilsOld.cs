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
}

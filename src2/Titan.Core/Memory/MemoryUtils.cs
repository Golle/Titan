using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Core.Memory
{
    public static unsafe class MemoryUtils
    {
        public static MemoryChunk<T> AllocateBlock<T>(uint count, bool zeroMemory = false) where T : unmanaged
        {
            static void OutOfMemoryException() => throw new OutOfMemoryException("Failed to allocate unmanaged memory.");
            var size = (uint)sizeof(T) * count;
            var ptr = (T*) Marshal.AllocHGlobal((int) size);
            if (zeroMemory)
            {
                Unsafe.InitBlock(ptr, 0, size);
            }
            
            if (ptr == null)
            {
                OutOfMemoryException();
            }
            return new MemoryChunk<T>(ptr, size);
        }

        public static MemoryChunk AllocateBlock(uint size, bool zeroMemory = false)
        {
            static void OutOfMemoryException() => throw new OutOfMemoryException("Failed to allocate unmanaged memory.");

            var ptr = (void*) Marshal.AllocHGlobal((int) size);
            if (ptr == null)
            {
                OutOfMemoryException();
            }

            if (zeroMemory)
            {
                Unsafe.InitBlock(ptr, 0, size);
            }
            return new MemoryChunk(ptr);
        }
    }
}

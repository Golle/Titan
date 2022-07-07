using System;

namespace Titan.Core.Memory;

public interface IMemoryPool : IDisposable
{
    T CreateAllocator<T>(uint size, bool initialize = false) where T : unmanaged, IMemoryAllocator<T>;
    unsafe T* GetPointer<T>(uint count = 1, bool initialize = false) where T : unmanaged;
    unsafe void* GetPointer(uint size, bool initialize = false);
}

using System;
using System.Collections.Generic;
using Titan.Core.Memory;

namespace Titan.ECS.Systems.Resources;

public unsafe class SharedResourceManager : ISharedResources
{
    private readonly MemoryChunk _memory;
    private readonly Dictionary<Type, uint> _allocatedTypes = new();
    private uint _offsetInBytes;

    public SharedResourceManager(uint sizeInBytes)
    {
        _memory = MemoryUtils.AllocateBlock(sizeInBytes);
    }

    /// <summary>
    /// Returns a pointer to the memory used by this type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T* GetMemoryForType<T>() where T : unmanaged
    {
        lock (_allocatedTypes)
        {
            var type = typeof(T);
            if (!_allocatedTypes.TryGetValue(type, out var offset))
            {
                var typeSize = sizeof(T);
                offset = _offsetInBytes + (uint)typeSize;
                if (offset > _memory.Size)
                {
                    throw new InvalidOperationException($"Trying to use more shared memory than allocated. Allocated: {_memory.Size} bytes. Current size: {offset}");
                }
                _allocatedTypes.Add(type, offset);
                _offsetInBytes = offset;
            }
            return (T*)((byte*)_memory.AsPointer() + offset);
        }
    }

    public void Dispose()
    {
        _memory.Free();
    }
}
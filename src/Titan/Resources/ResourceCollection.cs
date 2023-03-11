#nullable disable
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;

namespace Titan.Resources;

internal unsafe class ResourceCollection
{
    private ILinearAllocator _allocator;
    private void** _indices;
    private uint _maxTypes;
    private IMemoryManager _memoryManager;

    public bool Init(IMemoryManager memoryManager, uint maxResourceTypes, uint size)
    {
        var indicesSize = (uint)sizeof(void*) * maxResourceTypes;
        var indices = (void**)memoryManager.Alloc(indicesSize, true);

        if (indices == null)
        {
            Logger.Error<ResourceCollection>($"Failed to allocate {indicesSize} bytes of memory for the indices");
            return false;
        }

        if (!memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, size, out var allocator))
        {
            Logger.Error<ResourceCollection>($"Failed to create the {nameof(ILinearAllocator)} with {size} bytes.");
            memoryManager.Free(indices);
            return false;
        }
        _allocator = allocator;
        _indices = indices;
        _maxTypes = maxResourceTypes;
        _memoryManager = memoryManager;

        return true;
    }

    public void Shutdown()
    {
        if (_memoryManager != null)
        {
            _memoryManager.Free(_indices);
            _allocator.Destroy();
        }
    }

    public void AddManaged<T>(T value) where T : class
    {
        Debug.Assert(Contains<ObjectHandle<T>>() == false, $"Managed Resource of type {typeof(T).Name} has already been added to the collection");
        Add(new ObjectHandle<T>(value));
    }

    public void Add<T>(in T value = default) where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        Debug.Assert(id < _maxTypes);
        Debug.Assert(_indices[id] == null);
        _indices[id] = _allocator.Alloc<T>();
        *(T*)_indices[id] = value;
    }

    public ref ObjectHandle<T> GetManaged<T>() where T : class => ref Get<ObjectHandle<T>>();
    public ref T Get<T>() where T : unmanaged => ref *GetPointer<T>();
    public T* GetPointer<T>() where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        Debug.Assert(id < _maxTypes);
        Debug.Assert(_indices[id] != null, $"The type {typeof(T).FormattedName()} has not been registered.");
        return (T*)_indices[id];
    }
    
    public bool Contains<T>() where T : unmanaged
    {
        var id = ResourceId.Id<T>();
        Debug.Assert(id < _maxTypes);
        return _indices[id] != null;
    }
}

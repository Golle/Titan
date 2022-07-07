using System;
using Titan.Core.App;
using Titan.Core.Memory;

namespace Titan.ECS.SystemsV2;

public readonly unsafe struct SystemDescriptorCollection
{
    private readonly SystemDescriptor* _descriptors;
    private readonly uint* _count;
    private readonly uint _maxCount;
    private SystemDescriptorCollection(SystemDescriptor* descriptors, uint* count, uint maxCount)
    {
        _descriptors = descriptors;
        _maxCount = maxCount;
        _count = count;
        *_count = 0;
    }

    public void AddSystem<T>(Stage stage) where T : unmanaged, IStructSystem<T>
    {
        var next = *_count;
        if (next >= _maxCount)
        {
            throw new InvalidOperationException($"Max system count has been reached. {_maxCount}.");
        }
        _descriptors[next] = SystemDescriptor.Create<T>(stage);
        *_count += 1;
    }
    
    public static SystemDescriptorCollection Create(uint maxSystems, in MemoryPool pool) 
        => new(pool.GetPointer<SystemDescriptor>(maxSystems), pool.GetPointer<uint>(), maxSystems);

    internal ReadOnlySpan<SystemDescriptor> GetDescriptors() 
        => new(_descriptors, (int)*_count);
}

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Components;

namespace Titan.ECS.Entities;


internal struct EntityInfo
{
    public ComponentId Components;
}

internal struct EntityInfoRegistry : IResource
{
    private TitanArray<EntityInfo> _entityInfos;
    private ObjectHandle<IMemoryManager> _memoryManager;
    public bool Init(IMemoryManager memoryManager, uint maxEntites)
    {
        _entityInfos = memoryManager.AllocArray<EntityInfo>(maxEntites, true);
        if (!_entityInfos.IsValid)
        {
            Logger.Error<EntityInfoRegistry>($"Failed to allocate memory for {maxEntites} {nameof(EntityInfo)}s");
            return false;
        }
        _memoryManager = new ObjectHandle<IMemoryManager>(memoryManager);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref EntityInfo Get(in Entity entity)
    {
        Debug.Assert(entity.IsValid);
        Debug.Assert(entity.Id < _entityInfos.Length);
        return ref _entityInfos[entity];
    }

    public void Shutdown()
    {
        _memoryManager.Value.Free(ref _entityInfos);
        _memoryManager.Release();
    }
}

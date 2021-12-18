using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.ECS.Events;
using Titan.ECS.Worlds;

namespace Titan.ECS.Entities;

internal class EntityInfoManager
{
    private readonly MemoryChunk<EntityInfo> _entityInfos;
    private readonly uint _worldId;

    public EntityInfoManager(WorldConfiguration config)
    {
        Logger.Trace<EntityInfoManager>($"Creating entity info for {config.MaxEntities} entities.");
        _entityInfos = MemoryUtils.AllocateBlock<EntityInfo>(config.MaxEntities, true);
        _worldId = config.Id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref EntityInfo Get(in Entity entity) => ref _entityInfos[entity.Id];
    public ref EntityInfo this[in Entity entity]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _entityInfos[entity.Id];
    }

    public void Dispose() => _entityInfos.Free();

    public void Update()
    {
        foreach (ref readonly var @event in EventManager.GetEvents())
        {
            if (@event.Type == ComponentAddedEvent.Id)
            {
                ref readonly var e = ref @event.As<ComponentAddedEvent>();
                if (e.Entity.WorldId == _worldId)
                {
                    _entityInfos[e.Entity.Id].Components += e.Component;
                }
            }
            else if (@event.Type == ComponentBeingRemovedEvent.Id)
            {
                ref readonly var e = ref @event.As<ComponentBeingRemovedEvent>();
                if (e.Entity.WorldId == _worldId)
                {
                    _entityInfos[e.Entity.Id].Components -= e.Component;
                }
            }
        }
    }
}

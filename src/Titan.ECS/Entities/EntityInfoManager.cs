using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Worlds;

namespace Titan.ECS.Entities
{
    internal class EntityInfoManager
    {
        private readonly MemoryChunk<EntityInfo> _entityInfos;

        public EntityInfoManager(WorldConfiguration config)
        {
            Logger.Trace<EntityInfoManager>($"Creating entity info for {config.MaxEntities} entities.");
            _entityInfos = MemoryUtils.AllocateBlock<EntityInfo>(config.MaxEntities, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref EntityInfo Get(in Entity entity) => ref _entityInfos[entity.Id];
        public ref EntityInfo this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _entityInfos[entity.Id];
        }

        public void Dispose() => _entityInfos.Free();
    }
}

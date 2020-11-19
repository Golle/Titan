using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.ECS.World;

namespace Titan.ECS.Entities
{
    internal unsafe class EntityInfoRepository : IEntityInfoRepository
    {
        private EntityInfo* _entityInfos;

        public EntityInfoRepository(ECSConfiguration configuration)
        {
            _entityInfos = (EntityInfo*) Marshal.AllocHGlobal((int) (sizeof(EntityInfo) * configuration.MaxEntities));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly EntityInfo Get(in Entity entity) => ref _entityInfos[entity.Id];
        public ref readonly EntityInfo this[in Entity entity]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _entityInfos[entity.Id];
        }

        public void Dispose()
        {
            if (_entityInfos != null)
            {
                Marshal.FreeHGlobal((nint)_entityInfos);
                _entityInfos = null;
            }
        }
    }
}

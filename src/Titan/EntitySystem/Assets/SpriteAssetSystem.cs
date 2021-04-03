using Titan.Core.Logging;
using Titan.ECS.Assets;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;

namespace Titan.EntitySystem.Assets
{
    internal class ModelAssetSystem : SystemBase
    {
        private readonly IEntityFilter _filter;
        private readonly ReadOnlyStorage<UnmanagedAsset<MeshComponent>> _assets;

        public ModelAssetSystem(IWorld world, IEntityFilterManager entityFilterManager) : base(world)
        {
            _filter = entityFilterManager.Create(new EntityFilterConfiguration().With<UnmanagedAsset<MeshComponent>>().Not<MeshComponent>());
            _assets = GetRead<UnmanagedAsset<MeshComponent>>();
        }

        public override void OnUpdate()
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var asset = ref _assets.Get(entity);
                
                LOGGER.Trace("Woop, loaded the damn model");
                entity.AddComponent<MeshComponent>();
            }
        }
    }
}

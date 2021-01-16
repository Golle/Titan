using System.Numerics;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;

namespace Titan.EntitySystem
{
    // TODO: Use a sorted system base for this. This system needs to sort the entities based on the parent count, or a child could upate the transform before the parent causing a slight lag between parts.
    internal sealed class Transform3DSystem : SystemBase
    {
        private readonly IEntityFilter _filter;
        private readonly IEntityManager _entityManager;
        private readonly MutableStorage<Transform3D> _transform;
        
        public Transform3DSystem(IWorld world, IEntityManager entityManager, IEntityFilterManager entityFilterManager) 
            : base(world, priority: int.MinValue) // Set this to lowest priority, making it the last of the Transform3D systems to be executed. The WorldMatrix should then be up to date with the current frame
        {
            _filter = entityFilterManager.Create(new EntityFilterConfiguration().With<Transform3D>());
            _transform = GetMutable<Transform3D>();
            _entityManager = entityManager;
        }

        public override void OnUpdate()
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform.Get(entity);

                transform.ModelMatrix =
                    Matrix4x4.CreateScale(transform.Scale) *
                    Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                    Matrix4x4.CreateTranslation(transform.Position)
                    ;

                if (_entityManager.TryGetParent(entity, out var parent) && _transform.Contains(parent))
                {
                    transform.WorldMatrix = transform.ModelMatrix * _transform.Get(parent).WorldMatrix;
                }
                else
                {
                    transform.WorldMatrix = transform.ModelMatrix;
                }
            }
        }
    }
}

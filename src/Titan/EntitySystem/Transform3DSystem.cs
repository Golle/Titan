using Titan.ECS.Components;
using Titan.ECS.Registry;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;

namespace Titan.EntitySystem
{

    
    internal class Transform3DSystem : IEntitySystem
    {
        private readonly EntityFilter _filter;

        private readonly IComponentPool<Transform3D> _transform;
        private readonly IEntityRelationship _relationship;

        public Transform3DSystem(IWorld world)
        {
            //_filter = new EntityFilterBuilder(world)
            //    .With<Transform3D>()
            //    .Build();

            _transform = world.GetComponentPool<Transform3D>();
            _relationship = world.Relationship;

        }

        public void OnPreUpdate()
        {
            
        }

        public void OnUpdate(in TimeStep timeStep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform[entity];

                var parent = _relationship.GetParent(entity);
                if (!parent.IsNull())
                {
                    //TODO: The parent might not have a transform, the value will be undefined.
                    var parentTransform = _transform[_relationship.GetParent(entity)];
                }
            }
        }

        public void OnPostUpdate()
        {
        }

        public void Dispose()
        {
            _filter.Dispose();
        }
    }


}

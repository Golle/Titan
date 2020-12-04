using System.Numerics;
using Titan.ECS.Components;
using Titan.ECS.Entities;
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
        private readonly IEntityManager _entityManager;

        public Transform3DSystem(IWorld world)
        {
            
            //_filter = new EntityFilterBuilder(world)
            //    .With<Transform3D>()
            //    .Build();

            _transform = world.GetComponentPool<Transform3D>();
            _entityManager = world.EntityManager;
        }

        public void OnPreUpdate()
        {
            
        }

        public void OnUpdate(in TimeStep timeStep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform[entity];
                
                transform.ModelMatrix =
                    Matrix4x4.CreateScale(transform.Scale) *
                    Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                    Matrix4x4.CreateTranslation(transform.Position)
                    ;

                var parent = _entityManager.GetParent(entity);
                if (!parent.IsNull() && _transform.Contains(parent))
                { 
                    transform.WorldMatrix = transform.ModelMatrix * _transform[parent].WorldMatrix;
                }
                else
                {
                    transform.WorldMatrix = transform.ModelMatrix;
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

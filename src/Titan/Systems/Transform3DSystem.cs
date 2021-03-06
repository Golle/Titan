using System.Numerics;
using Titan.Components;
using Titan.ECS.Systems;

namespace Titan.Systems
{
    internal class Transform3DSystem : EntitySystem
    {
        private MutableStorage<Transform3D> _transform;
        private EntityFilter _filter;

        protected override void Init()
        {
            _transform = GetMutable<Transform3D>();
            _filter = CreateFilter(new EntityFilterConfiguration().With<Transform3D>());
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform.Get(entity);

                transform.WorldMatrix = transform.ModelMatrix =
                    Matrix4x4.CreateScale(transform.Scale) *
                    Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                    Matrix4x4.CreateTranslation(transform.Position)
                    ;
            }
        }

    }
}

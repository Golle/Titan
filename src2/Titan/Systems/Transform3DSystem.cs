using Titan.Components;
using Titan.ECS.Systems;

namespace Titan.Systems
{
    internal class Transform3DSystem : EntitySystem
    {
        private ReadOnlyStorage<Transform3D> _transform;
        private EntityFilter _filter;

        protected override void Init()
        {
            _transform = GetReadOnly<Transform3D>();
            _filter = CreateFilter(new EntityFilterConfiguration().With<Transform3D>());
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                //Logger.Warning<Transform3DSystem>("Lol " + transform.Position);
            }
        }

    }
}

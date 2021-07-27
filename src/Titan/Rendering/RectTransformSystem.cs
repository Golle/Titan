using System.Numerics;
using Titan.Components;
using Titan.ECS.Systems;

namespace Titan.Rendering
{
    public class RectTransformSystem : EntitySystem
    {
        private EntityFilter _filter;
        private MutableStorage<RectTransform> _transform;
        
        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>());

            _transform = GetMutable<RectTransform>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform.Get(entity);
                if (EntityManager.TryGetParent(entity, out var parent) && _transform.Contains(parent))
                {
                    ref readonly var parentTransform = ref _transform.Get(parent);
                    // Increase z-index with 0.5 per parent so it gets rendered in the correct order
                    transform.Position = parentTransform.Position + new Vector3(transform.Offset, transform.ZIndex + 0.5f);
                }
                else
                {
                    transform.Position = new Vector3(transform.Offset, transform.ZIndex);
                }
            }
        }
    }
}

using Titan.ECS.Systems;
using Titan.Input;
using Titan.UI.Components;

namespace Titan.UI.Systems
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

                if (InputManager.LeftMouseButtonDown)
                {
                    transform.Size.Width += 1;
                    transform.Size.Height += 1;
                }

                if (InputManager.RightMouseButtonDown)
                {
                    transform.Size.Height -= 1;
                    transform.Size.Width -= 1;
                }
                

                if (EntityManager.TryGetParent(entity, out var parent) && _transform.Contains(parent))
                {
                    ref readonly var parentTransform = ref _transform.Get(parent);
                    // Increase z-index with 0.5 per parent so it gets rendered in the correct order
                    transform.Position = parentTransform.Position + transform.Offset;
                    transform.AbsoluteZIndex = transform.ZIndex + parentTransform.AbsoluteZIndex + 1;

                    //TODO: not sure how we should do this. use constraints maybe?
                    if (transform.Size.Height == 0 && transform.Size.Width == 0)
                    {
                        transform.Size = parentTransform.Size;
                    }
                }
                else
                {
                    transform.Position = transform.Offset;
                    transform.AbsoluteZIndex = transform.ZIndex;
                }
                
            }
        }
    }
}

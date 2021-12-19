using System.Numerics;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.Input;
using Titan.UI.Components;

namespace Titan.UI
{
    public record UIConfiguration(uint MaxSprites = 1000, uint MaxComponents = 1000);
    public class TestDragAndDropSystem : EntitySystem
    {
        private EntityFilter _filter;
        private MutableStorage<RectTransform> _transform;
        private ReadOnlyStorage<InteractableComponent> _interactable;

        protected override void Init(IServiceCollection services)
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<InteractableComponent>());

            _transform = GetMutable<RectTransform>();
            _interactable = GetReadOnly<InteractableComponent>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var interactable = ref _interactable.Get(entity);
                ref var transform = ref _transform.Get(entity);
                if ((interactable.MouseState & MouseState.Down) != 0)
                {
                    transform.Offset += new Vector2(-InputManager.MouseDeltaPosition.X, -InputManager.MouseDeltaPosition.Y);
                }
            }
        }
    }
}

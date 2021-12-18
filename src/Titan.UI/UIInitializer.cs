using System.Numerics;
using Titan.Assets;
using Titan.Core.Services;
using Titan.ECS;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.Input;
using Titan.UI.Animation;
using Titan.UI.Components;
using Titan.UI.Systems;

namespace Titan.UI
{
    public record UIConfiguration(uint MaxSprites = 1000, uint MaxComponents = 1000);
    public static class UIInitializer
    {
        public static WorldBuilder WithDefaultUI(this WorldBuilder builder, UIConfiguration config) =>
            builder
                .WithComponent<AssetComponent<SpriteComponent>>(ComponentPoolTypes.DynamicPacked, count: 1)
                .WithComponent<AssetComponent<TextComponent>>(ComponentPoolTypes.DynamicPacked, count: 1)
                .WithComponent<SpriteComponent>(count: config.MaxComponents)
                .WithComponent<RectTransform>(count: config.MaxComponents)
                .WithComponent<InteractableComponent>(count: config.MaxComponents)
                .WithComponent<TextComponent>(count: config.MaxComponents)

                .WithSystem<SpriteLoaderSystem>()
                .WithSystem<TextLoaderSystem>()
                .WithSystem<UISpriteRenderSystem>()
                .WithSystem<UITextRenderSystem>()
                .WithSystem<TextUpdateSystem>()
                .WithSystem<RectTransformSystem>()
                .WithSystem<InteractableSystem>()


                //.WithSystem(new AnimateTranslationSystem())
                .WithComponent<AnimateTranslation>(count:100)
                
                //.WithSystem(new TestDragAndDropSystem())
            ;
    }
    
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

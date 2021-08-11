using System.Numerics;
using Titan.Assets;
using Titan.ECS;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Atlas;
using Titan.Input;
using Titan.UI.Components;
using Titan.UI.Rendering;
using Titan.UI.Systems;

namespace Titan.UI
{
    public record UIConfiguration(uint MaxSprites = 100, uint MaxComponents = 1000);
    public static class UIInitializer
    {
        public static WorldBuilder WithDefaultUI(this WorldBuilder builder, UIConfiguration config, UIRenderQueue renderQueue, AssetsManager assetsManager, AtlasManager atlasManager) =>
            builder
                .WithComponent<AssetComponent<SpriteComponent>>(count: config.MaxSprites)
                .WithComponent<SpriteComponent>(count: config.MaxComponents)
                .WithComponent<RectTransform>(count: config.MaxComponents)
                .WithComponent<InteractableComponent>(count: config.MaxComponents)

                .WithSystem(new SpriteLoaderSystem(assetsManager))
                .WithSystem(new UIRenderSystem(renderQueue, atlasManager))
                .WithSystem(new RectTransformSystem())
                .WithSystem(new InteractableSystem())
                .WithSystem(new TestDragAndDropSystem())
            ;
    }



    public class TestDragAndDropSystem : EntitySystem
    {
        private EntityFilter _filter;
        private MutableStorage<RectTransform> _transform;
        private ReadOnlyStorage<InteractableComponent> _interactable;

        protected override void Init()
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

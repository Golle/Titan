using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Atlas;
using Titan.Graphics.Rendering.Sprites;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    public class UISpriteRenderSystem : EntitySystem
    {
        private SpriteRenderQueue _renderQueue;
        private AtlasManager _atlasManager;
        private EntityFilter _spriteFilter;
        private ReadOnlyStorage<RectTransform> _transform;
        private ReadOnlyStorage<SpriteComponent> _sprite;
        private ReadOnlyStorage<InteractableComponent> _interactable;

        protected override void Init(IServiceCollection services)
        {
            _spriteFilter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<SpriteComponent>());

            _transform = GetReadOnly<RectTransform>();
            _sprite = GetReadOnly<SpriteComponent>();
            _interactable = GetReadOnly<InteractableComponent>();

            _renderQueue = services.Get<SpriteRenderQueue>();
            _atlasManager = services.Get<AtlasManager>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _spriteFilter.GetEntities()) // TODO: this should be sorted by parent count
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref readonly var sprite = ref _sprite.Get(entity);
                ref readonly var atlas = ref _atlasManager.Access(sprite.TextureAtlas);
                var coordinates = atlas.Get(sprite.TextureIndex);
                var type = atlas.Type(sprite.TextureIndex);

                var color = sprite.Color;

                switch (type)
                {
                    case SpriteType.Normal:
                        _renderQueue.Add(transform.AbsolutePosition, transform.AbsoluteZIndex, transform.Size, atlas.Texture, coordinates, color);
                        break;
                    case SpriteType.Slice:
                        _renderQueue.AddNineSlice(transform.AbsolutePosition, transform.AbsoluteZIndex, transform.Size, atlas.Texture, coordinates, color, sprite.Margins);
                        break;
                }
            }
        }

        
    }
}

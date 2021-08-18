using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.Graphics;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Components;
using Titan.UI.Rendering;

namespace Titan.UI.Systems
{
    public class UISpriteRenderSystem : EntitySystem
    {
        private readonly UIRenderQueue _renderQueue;
        private readonly AtlasManager _atlasManager;
        private EntityFilter _spriteFilter;
        private ReadOnlyStorage<RectTransform> _transform;
        private ReadOnlyStorage<SpriteComponent> _sprite;
        private ReadOnlyStorage<InteractableComponent> _interactable;

        public UISpriteRenderSystem(UIRenderQueue renderQueue, AtlasManager atlasManager)
        {
            _renderQueue = renderQueue;
            _atlasManager = atlasManager;
        }

        protected override void Init()
        {
            _spriteFilter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<SpriteComponent>());

            _transform = GetReadOnly<RectTransform>();
            _sprite = GetReadOnly<SpriteComponent>();
            _interactable = GetReadOnly<InteractableComponent>();
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

                var color = GetColor(entity);

                switch (type)
                {
                    case SpriteType.Normal:
                        _renderQueue.Add(transform.Position, transform.AbsoluteZIndex, transform.Size, atlas.Texture, coordinates, color);
                        break;
                    case SpriteType.Slice:
                        _renderQueue.AddNineSlice(transform.Position, transform.AbsoluteZIndex, transform.Size, atlas.Texture, coordinates, color, sprite.Margins);
                        break;
                }
            }
        }

        private Color GetColor(Entity entity)
        {
            // Temporary, just to test out hover effect
            var color = Color.White;
            if (_interactable.Contains(entity))
            {
                var state = _interactable.Get(entity).MouseState;
                if ((state & MouseState.Down) > 0)
                {
                    color = Color.Red;
                }
                else if ((state & MouseState.Up) > 0)
                {
                    color = Color.Black;
                }
                else if ((state & MouseState.Hover) > 0)
                {
                    color = Color.Blue;
                }
            }

            return color;
        }
    }
}

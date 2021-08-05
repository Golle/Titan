using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Components;
using Titan.UI.Rendering;

namespace Titan.UI.Systems
{
    public class UIRenderSystem : EntitySystem
    {
        private readonly UIRenderQueue _renderQueue;
        private readonly AtlasManager _atlasManager;
        private EntityFilter _filter;
        private ReadOnlyStorage<RectTransform> _transform;
        private ReadOnlyStorage<SpriteComponent> _sprite;

        public UIRenderSystem(UIRenderQueue renderQueue, AtlasManager atlasManager)
        {
            _renderQueue = renderQueue;
            _atlasManager = atlasManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<SpriteComponent>());

            _transform = GetReadOnly<RectTransform>();
            _sprite = GetReadOnly<SpriteComponent>();
        }


        protected override void OnPreUpdate()
        {
            _renderQueue.Begin();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities()) // TODO: this should be sorted by parent count
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref readonly var sprite = ref _sprite.Get(entity);
                ref readonly var atlas = ref _atlasManager.Access(sprite.TextureAtlas);
                ref readonly var coordinates = ref atlas.Get(sprite.TextureIndex);
                _renderQueue.Add(transform.Position, transform.AbsoluteZIndex, transform.Size, atlas.Texture, coordinates);
            }
        }

        protected override void OnPostUpdate()
        {
            _renderQueue.End();
        }
    }
}

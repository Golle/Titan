using Titan.Components;
using Titan.ECS.Systems;
using Titan.UI.Rendering;

namespace Titan.Systems
{
    internal class UIRenderSystem : EntitySystem
    {
        private readonly UIRenderQueue _renderQueue;
        private EntityFilter _filter;
        private ReadOnlyStorage<RectTransform> _transform;
        private ReadOnlyStorage<Sprite> _sprite;

        public UIRenderSystem(UIRenderQueue renderQueue)
        {
            _renderQueue = renderQueue;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<Sprite>());

            _transform = GetReadOnly<RectTransform>();
            _sprite = GetReadOnly<Sprite>();
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
                
                _renderQueue.Add(transform.Position, transform.Size, sprite.Texture);
            }
        }

        protected override void OnPostUpdate()
        {
            _renderQueue.End();
        }
    }
}

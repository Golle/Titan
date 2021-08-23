using System;
using System.Numerics;
using Titan.ECS.Systems;
using Titan.UI.Components;

namespace Titan.UI.Debugging
{
    public class UIBoundingBoxDebugSystem : EntitySystem
    {
        private readonly BoundingBoxRenderQueue _renderQueue;
        private EntityFilter _filter;
        private ReadOnlyStorage<RectTransform> _transform;

        public UIBoundingBoxDebugSystem(BoundingBoxRenderQueue renderQueue)
        {
            _renderQueue = renderQueue;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>());
            _transform = GetReadOnly<RectTransform>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            Span<Vector3> positions = stackalloc Vector3[4];
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);

                positions[0] = new Vector3(transform.AbsolutePosition, 1f);
                positions[1] = new Vector3(transform.AbsolutePosition, 1f) + Vector3.UnitX*transform.Size.Width;
                positions[2] = new Vector3(transform.AbsolutePosition, 1f) + Vector3.UnitX * transform.Size.Width + Vector3.UnitY * transform.Size.Height;
                positions[3] = new Vector3(transform.AbsolutePosition, 1f) + Vector3.UnitY * transform.Size.Height;
                _renderQueue.Add(positions);
            }
        }
    }
}

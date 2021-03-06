using System.Numerics;
using Titan.Components;
using Titan.ECS.Systems;
using Titan.Graphics;

namespace Titan.Systems
{
    internal class CameraSystem : EntitySystem
    {
        private readonly GraphicsSystem _graphicsSystem;

        // TODO: read this from configuration?
        private static readonly Vector3 Forward = Vector3.UnitZ;
        private static readonly Vector3 Up = Vector3.UnitY;

        private MutableStorage<CameraComponent> _camera;
        private ReadOnlyStorage<Transform3D> _transform;
        private EntityFilter _filter;
        public CameraSystem(GraphicsSystem graphicsSystem)
        {
            _graphicsSystem = graphicsSystem; // TODO: replace with publish event
        }

        protected override void Init()
        {
            _camera = GetMutable<CameraComponent>();
            _transform = GetReadOnly<Transform3D>();
            _filter = CreateFilter(new EntityFilterConfiguration().With<CameraComponent>().With<Transform3D>());
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var camera = ref _camera.Get(entity);
                // Go through components until the first camera is found
                if (camera.Active)
                {
                    ref readonly var transform = ref _transform.Get(entity);
                    var forward = Vector3.Transform(Forward, transform.Rotation);
                    var up = Vector3.Transform(Up, transform.Rotation);
                    camera.View = Matrix4x4.CreateLookAt(transform.Position, transform.Position + forward, up);
                    camera.ViewProjection = camera.View * camera.Projection;
                    _graphicsSystem.SetCamera(camera.View, camera.ViewProjection);
                    break;// TODO : add some property that will determine the "main" camera
                }
            }
        }
    }
}

using System.Numerics;
using Titan.Core.Logging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;

namespace Titan.EntitySystem
{
    internal class CameraSystem : SystemBase
    {
        // TODO: read this from configuration?
        private static readonly Vector3 Forward = Vector3.UnitZ;
        private static readonly Vector3 Up = Vector3.UnitY;

        private readonly ReadOnlyStorage<Transform3D> _transform;
        private readonly MutableStorage<CameraComponent> _camera;
        private readonly IEntityFilter _filter;
        
        private Entity _activeCamera;

        public CameraSystem(IWorld world, IEntityFilterManager entityFilterManager) 
            : base(world)
        {
            _camera = GetMutable<CameraComponent>();
            _transform = GetRead<Transform3D>();
            _filter = entityFilterManager.Create(new EntityFilterConfiguration().With<CameraComponent>().With<Transform3D>());
        }

        public override void OnPreUpdate()
        {
            _activeCamera = Entity.Null;
        }

        public override void OnUpdate()
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref var camera = ref _camera.Get(entity);

                // Go through components until the first camera is found
                if (camera.Active)
                {
                    var forward = Vector3.Transform(Forward, transform.Rotation);
                    var up = Vector3.Transform(Up, transform.Rotation);
                    camera.View = Matrix4x4.CreateLookAt(transform.Position, transform.Position + forward, up);
                    camera.ViewProjection = camera.View * camera.Projection;
                    _activeCamera = entity;
                    break;// TODO : add some property that will determine the "main" camera
                }
            }
        }

        public override void OnPostUpdate()
        {
            if (!_activeCamera.IsNull())
            {
                ref readonly var camera = ref _camera.Get(_activeCamera);
            }
#if DEBUG
            else
            {
                LOGGER.Warning("No active camera set, nothing will be rendered.");
            }

#endif
            // TODO: set the camera in the renderer
        }
    }
}

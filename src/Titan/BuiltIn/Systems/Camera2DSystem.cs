using Titan.BuiltIn.Components;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Titan.BuiltIn.Systems;

internal struct Camera2DSystem : ISystem
{
    private EntityQuery _entities;
    private MutableStorage<Camera2D> _camera;
    private ReadOnlyStorage<Transform2D> _transform2D;

    public void Init(in SystemInitializer init)
    {
        _camera = init.GetMutableStorage<Camera2D>();
        _transform2D = init.GetReadOnlyStorage<Transform2D>();

        _entities = init.CreateQuery(new EntityQueryArgs().With<Camera2D>().With<Transform2D>());
    }

    public void Update()
    {
        foreach (ref readonly var entity in _entities)
        {
            ref var camera = ref _camera[entity];
            ref readonly var transform = ref _transform2D[entity];

            camera.Position.X = transform.WorldPosition.X;
            camera.Position.Y = transform.WorldPosition.Y;

            //NOTE(Jens): maybe we want some enum for configuration on the camera that decides how the relative scale etc should work. For example if we have a character that we want to Scale down to 50% of the original size, that would also scale the viewport/camera if we use WorldScale.
            camera.Scale = transform.Scale;
        }
    }
}

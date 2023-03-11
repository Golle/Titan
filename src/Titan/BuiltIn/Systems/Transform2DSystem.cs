using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.BuiltIn.Components;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Titan.BuiltIn.Systems;

internal struct Transform2DSystem : ISystem
{
    private EntityQuery _query;
    private MutableStorage<Transform2D> _transform;
    private EntityManager _entityManager;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _query = init.CreateQuery(new EntityQueryArgs().With<Transform2D>());
        _transform = init.GetMutableStorage<Transform2D>();
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Update()
    {
        //NOTE(Jens): look into if we can do this with SIMD
        foreach (ref readonly var entity in _query)
        {
            ref var transform = ref _transform[entity];
            //NOTE(Jens): Contains will always return true for Sparse pools. We might want to track entities in some other way for those pools?
            if (_entityManager.TryGetParent(entity, out var parent) && _transform.Contains(parent))
            {
                //NOTE(Jens): How do we implement Rotation? we need some kind of matrix for that if we want the rotation to affect the entire object and all children (basically updating the world position and not just the rotation itself)

                ref readonly var parentRotation = ref _transform[parent].WorldRotation;
                var localPosition = transform.Position;
                if (parentRotation != 0 && localPosition != Vector2.Zero)
                {
                    //NOTE(Jens): Not sure why we have to inverse the parent rotation to match the rotation in the shader.
                    var (sin, cos) = MathF.SinCos(-parentRotation);
                    var x = localPosition.X;
                    var y = localPosition.Y;
                    //NOTE(Jens): maybe this can be extracted into a some custom matrix?
                    localPosition.X = x * cos - y * sin;
                    localPosition.Y = x * sin + y * cos;
                }

                transform.WorldPosition = localPosition * _transform[parent].WorldScale + _transform[parent].WorldPosition;
                transform.WorldScale = transform.Scale * _transform[parent].WorldScale;
                transform.WorldRotation = transform.Rotation + _transform[parent].WorldRotation;
            }
            else
            {
                transform.WorldPosition = transform.Position;
                transform.WorldScale = transform.Scale;
                transform.WorldRotation = transform.Rotation;
            }
        }
    }
}

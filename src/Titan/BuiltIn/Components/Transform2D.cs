using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Maths;
using Titan.ECS.Components;
using Titan.Setup;

namespace Titan.BuiltIn.Components;

//NOTE(Jens): should we use Transform3D for all? I think this might be a good idea at some point, but for now we use 2 separate ones.
public struct Transform2D : IComponent, IDefault<Transform2D>
{
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;

    internal Vector2 WorldPosition;
    internal Vector2 WorldScale;
    internal float WorldRotation;
    [UnscopedRef]
    public readonly ref readonly Vector2 GetWorldPosition() => ref WorldPosition;
    public Transform2D(in Vector2 position)
        : this(position, Vector2.One)
    {
    }

    public Transform2D(in Vector2 position, in Vector2 scale)
        : this(position, scale, 0f)
    {
    }

    public Transform2D(in Vector2 position, in Vector2 scale, float rotation)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRotation(float radians) => Rotation = radians;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetEulerRotation(float degrees) => Rotation = Converters.DegressToRadians(degrees);

    public static Transform2D Default
    {
        get
        {
            Unsafe.SkipInit(out Transform2D transform);
            transform.Position = Vector2.Zero;
            transform.Scale = Vector2.One;
            transform.Rotation = 0;
            return transform;
        }
    }
}

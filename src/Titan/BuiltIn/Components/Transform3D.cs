using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.Setup;

namespace Titan.BuiltIn.Components;

public struct Transform3D : IComponent, IDefault<Transform3D>
{
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
    public static Transform3D Default
    {
        get
        {
            Unsafe.SkipInit(out Transform3D transform);
            transform.Position = Vector3.Zero;
            transform.Rotation = Quaternion.Zero;
            transform.Scale = Vector3.One;
            return transform;
        }
    }
}

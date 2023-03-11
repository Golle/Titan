using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Maths;
using Titan.ECS.Components;
using Titan.Setup;

namespace Titan.BuiltIn.Components;

public struct TransformRect : IComponent, IDefault<TransformRect>
{
    public Vector2 Position;
    public Size Size;

    public static TransformRect Default
    {
        get
        {
            Unsafe.SkipInit(out TransformRect transform);
            transform.Position = Vector2.Zero;
            transform.Size = Size.Zero;
            return transform;
        }
    }
}

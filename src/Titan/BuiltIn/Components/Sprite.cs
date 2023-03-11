using System.Numerics;
using Titan.Assets;
using Titan.Core;
using Titan.Core.Maths;
using Titan.ECS.Components;
using Titan.Setup;

namespace Titan.BuiltIn.Components;

public struct Sprite : IComponent, IDefault<Sprite>
{
    //public Vector2 TextureCoordinates;
    //public Size Size;
    public Color Color;
    public Rectangle SourceRect;
    public Vector2 Pivot;
    public Handle<Asset> Asset;
    public short Layer;

    public static Sprite Default => new()
    {
        Asset = Handle<Asset>.Invalid,
        Color = Color.White,
        Pivot = Vector2.One * 0.5f,
        Layer = 0,
        SourceRect = default
    };
}

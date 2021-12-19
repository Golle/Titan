using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Components;

[StructLayout(LayoutKind.Sequential)]
public struct Transform2D
{
    public Vector2 Position;
    public Vector2 Scale;
    public Quaternion Rotation;

    public static readonly Transform2D Default = new()
    {
        Position = Vector2.Zero,
        Rotation = Quaternion.Identity,
        Scale = Vector2.One
    };


    public static Transform2D Create(in Vector2 position) =>
        new()
        {
            Position = position,
            Rotation = Quaternion.Identity,
            Scale = Vector2.One
        };
}

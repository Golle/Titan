using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Components;

[StructLayout(LayoutKind.Sequential)]
public struct Transform3DComponent
{
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;

    internal Matrix4x4 ModelMatrix;
    internal Matrix4x4 WorldMatrix;

    public static readonly Transform3DComponent Default = new()
    {
        Position = Vector3.Zero,
        Rotation = Quaternion.Identity,
        Scale = Vector3.One
    };
}

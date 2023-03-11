using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Titan.BuiltIn.Components;

[StructLayout(LayoutKind.Sequential)]
public struct Camera2D : IComponent
{
    public Color ClearColor;
    public SizeF Size;

    internal Vector2 Position;
    internal Vector2 Scale;
}

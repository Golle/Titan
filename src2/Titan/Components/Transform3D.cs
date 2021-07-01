using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Transform3D
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        internal Matrix4x4 ModelMatrix;
        internal Matrix4x4 WorldMatrix;

        public static readonly Transform3D Default = new()
        {
            Position = Vector3.Zero,
            Rotation = Quaternion.Identity,
            Scale = Vector3.One
        };
    }
}

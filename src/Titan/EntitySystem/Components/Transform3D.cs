using System.Numerics;

namespace Titan.EntitySystem.Components
{
    public struct Transform3D
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public Matrix4x4 ModelMatrix;
        public Matrix4x4 WorldMatrix;

        // TODO: dirty flag? or use a list of "changed" components?
    }
}
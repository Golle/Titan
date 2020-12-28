using System.Numerics;

namespace Titan.EntitySystem.Components
{
    public struct Transform2D
    {
        public Vector2 Position;
        public Vector2 Scale;
        public Quaternion Rotation;

        public Vector2 WorldPosition;

    }
}

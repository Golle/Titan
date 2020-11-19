using System.Numerics;
using Titan.ECS;

namespace Titan.EntitySystem.Components
{
    [Component]
    public partial struct Transform2D
    {
        public Vector2 Position;
        public Vector2 Scale;
        public Quaternion Rotation;

        public Vector2 WorldPosition;

    }
}

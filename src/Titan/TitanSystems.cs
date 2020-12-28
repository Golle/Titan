using Titan.EntitySystem;

namespace Titan
{
    public static class TitanSystems
    {
        public static readonly string Transform3D = nameof(Transform3DSystem);
        public static readonly string Transform2D = nameof(Transform3DSystem); // TODO: change when there's a Transform2DSystem
    }
}

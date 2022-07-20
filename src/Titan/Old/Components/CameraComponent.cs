using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core.Math;

namespace Titan.Old.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CameraComponent
    {
        public bool Active;
        internal int Width;
        internal int Height;
        internal float Near;
        internal float Far;


        internal Matrix4x4 View;
        internal Matrix4x4 Projection;
        internal Matrix4x4 ViewProjection;

        public static CameraComponent CreatePerspective(int width, int height, float near = 0.5f, float far = 1000f, bool active = true) =>
            new()
            {
                Width = width,
                Height = height,
                Near = near,
                Far = far,
                Projection = MatrixExtensions.CreatePerspectiveLH(1f, height / (float)width, near, far),
                Active = active
            };
    }
}

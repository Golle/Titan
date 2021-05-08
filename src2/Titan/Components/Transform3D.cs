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
    }
}

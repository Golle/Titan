using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Meshes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SubMesh
    {
        public int StartIndex;
        public int Count;
        public int MaterialIndex;
        public Vector3 Min;
        public Vector3 Max;
    }
}

using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.Graphics.Meshes
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normals;
        public Vector2 Texture;
    }
}

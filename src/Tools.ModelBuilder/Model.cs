using System.Numerics;
using System.Runtime.InteropServices;

namespace Tools.ModelBuilder
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TexturedVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
    }

    
}

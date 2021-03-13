using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.GraphicsV2.Resources
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SubMeshData
    {
        public uint StartIndex;
        public uint Count;
        public int MaterialIndex;
        public Vector3 Min;
        public Vector3 Max;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexData
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MeshDataHeader
    {
        public int Indicies;
        public int IndexSize;
        public int Vertices;
        public int VertexSize;
        public int SubMeshes;
        public int SubMeshSize;
    }
}

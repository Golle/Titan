using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MeshDescriptor
    {
        public uint VertexSize;
        public uint NumberOfVertices;
        public uint NumberOfIndices;
        public int NumberOfSubmeshes;
    }
}

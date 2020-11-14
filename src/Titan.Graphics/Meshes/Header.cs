using System.Runtime.InteropServices;

namespace Titan.Graphics.Meshes
{
    [StructLayout(LayoutKind.Sequential, Size = 256, Pack = 2)]
    internal struct Header
    {
        public ushort Version;
        public ushort NumberOfChunks;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct ChunkHeader
    {
        public uint VertexSize;
        public uint VertexCount;
        public uint IndexSize;
        public uint IndexCount;
        public uint SubMeshCount;
    }
}

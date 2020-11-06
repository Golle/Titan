using System.Runtime.InteropServices;

namespace Titan.Graphics.Meshes
{
    [StructLayout(LayoutKind.Sequential, Size = 256)]
    internal struct Header
    {
        public ushort Version;
        public uint VertexSize;
        public uint VertexCount;
        public uint IndexSize;
        public uint IndexCount;
        public uint SubMeshCount;
    }
}

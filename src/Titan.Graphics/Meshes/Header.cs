using System.Runtime.InteropServices;

namespace Titan.Graphics.Meshes
{
    [StructLayout(LayoutKind.Sequential, Size = 256)]
    internal struct Header
    {
        public ushort Version;
        public int VertexSize;
        public int VertexCount;
        public int IndexSize;
        public int IndexCount;
        public int SubMeshCount;
    }
}

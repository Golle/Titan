using System.Runtime.InteropServices;

namespace Titan.AssetConverter.Exporter
{
    [StructLayout(LayoutKind.Sequential, Size = 256, Pack = 2)]
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

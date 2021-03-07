using Titan.Core.Common;
using Titan.GraphicsV2.D3D11.Buffers;

namespace Titan.GraphicsV2.Resources
{
    internal struct Model3D
    {
        internal Handle<Buffer> VertexBuffer;
        internal Handle<Buffer> IndexBuffer;
        internal int SubMeshCount;
        internal SubMesh[] SubMeshes; //TODO: extra heap allocation, might be possible to replace this with a  fixed buffer (allocate all memory + header and cast it to a Model3D pointer)
    }
    internal struct SubMesh
    {
        internal int Start;
        internal int Count;
    }
}

using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Loaders.Materials;

namespace Titan.Graphics.Loaders.Models
{
    public struct Mesh
    {
        public Handle<ResourceBuffer> VertexBuffer;
        public Handle<ResourceBuffer> IndexBuffer;

        public MemoryChunk<Submesh> Submeshes;
    }
    public struct Submesh
    {
        public uint StartIndex;
        public uint Count;
        public Handle<Material> Material;
    }
}

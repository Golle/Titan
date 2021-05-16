using Titan.Core;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Assets.Models
{
    public struct Mesh
    {
        public Handle<Buffer> VertexBuffer;
        public Handle<Buffer> IndexBuffer;
        public Submesh[] Submeshes;
    }
}

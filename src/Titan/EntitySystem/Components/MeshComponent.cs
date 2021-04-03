using Titan.Core.Common;
using Titan.GraphicsV2.D3D11.Buffers;

namespace Titan.EntitySystem.Components
{
    internal struct MeshComponent
    {
        public Handle<Buffer> VertexBuffer;
        public Handle<Buffer> IndexBuffer;
    }
}

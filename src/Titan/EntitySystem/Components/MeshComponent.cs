using Titan.Core.Common;
using Titan.Graphics.Resources;

namespace Titan.EntitySystem.Components
{
    internal struct MeshComponent
    {
        public Handle<VertexBuffer> VertexBuffer;
        public Handle<IndexBuffer> IndexBuffer;
    }
}

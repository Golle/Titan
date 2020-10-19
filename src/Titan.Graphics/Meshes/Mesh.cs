using System;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Graphics.Meshes
{
    public record Mesh(IVertexBuffer VertexBuffer, IIndexBuffer IndexBuffer, SubMesh[] SubSets) : IDisposable
    {
        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}

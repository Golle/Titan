using Titan.Graphics.Resources;

namespace Titan.Graphics.Meshes
{
    public record Mesh(VertexBufferHandle VertexBufferHandle, IndexBufferHandle IndexBufferHandle, SubMesh[] SubSets)
    {
        //public void Dispose()
        //{
        //    VertexBuffer.Dispose();
        //    IndexBuffer.Dispose();
        //}
    }
}

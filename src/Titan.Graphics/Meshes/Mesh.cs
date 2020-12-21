using Titan.Graphics.Resources;

namespace Titan.Graphics.Meshes
{
    public record Mesh(Handle<VertexBuffer> VertexBuffer, Handle<IndexBuffer> IndexBuffer, SubMesh[] SubSets);
}

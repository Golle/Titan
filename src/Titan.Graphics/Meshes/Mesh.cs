using Titan.Graphics.Resources;

namespace Titan.Graphics.Meshes
{
    public record Mesh(bool HasBumpMap, VertexBufferHandle VertexBufferHandle, IndexBufferHandle IndexBufferHandle, SubMesh[] SubSets);
}

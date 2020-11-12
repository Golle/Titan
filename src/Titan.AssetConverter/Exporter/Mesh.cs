namespace Titan.AssetConverter.Exporter
{
    internal class Mesh
    {
        public Vertex[] Vertices { get; }
        public int[] Indices { get; }
        public SubMesh[] SubMeshes { get; }
        public Mesh(Vertex[] vertices, int[] indices, SubMesh[] subMeshes)
        {
            Vertices = vertices;
            Indices = indices;
            SubMeshes = subMeshes;
        }
    }
}

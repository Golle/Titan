namespace Titan.AssetConverter.Exporter
{
    internal class Mesh<T> where T : unmanaged
    {
        public T[] Vertices { get; }
        public int[] Indices { get; }
        public SubMesh[] SubMeshes { get; }
        public Mesh(T[] vertices, int[] indices, SubMesh[] subMeshes)
        {
            Vertices = vertices;
            Indices = indices;
            SubMeshes = subMeshes;
        }
    }
}

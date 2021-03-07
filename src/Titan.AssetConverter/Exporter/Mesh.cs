using System;

namespace Titan.AssetConverter.Exporter
{
    internal class Mesh<T> where T : unmanaged
    {
        public T[] Vertices { get; }
        public ReadOnlyMemory<SubMesh> SubMeshes { get; }
        public ReadOnlyMemory<int> Indices { get; }

        public Mesh(T[] vertices, ReadOnlyMemory<int> indices, ReadOnlyMemory<SubMesh> subMeshes)
        {
            Vertices = vertices;
            Indices = indices;
            SubMeshes = subMeshes;
        }
    }
}

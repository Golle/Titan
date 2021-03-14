using System;
using Titan.GraphicsV2.Resources;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.AssetConverter.Exporter
{
    internal class Mesh<T> where T : unmanaged
    {
        public ReadOnlyMemory<T> Vertices { get; }
        public ReadOnlyMemory<SubMeshData> SubMeshes { get; }
        public ReadOnlyMemory<int> Indices { get; }
        public Mesh(ReadOnlyMemory<T> vertices, ReadOnlyMemory<int> indices, ReadOnlyMemory<SubMeshData> subMeshes)
        {
            Vertices = vertices;
            Indices = indices;
            SubMeshes = subMeshes;
        }
    }
}

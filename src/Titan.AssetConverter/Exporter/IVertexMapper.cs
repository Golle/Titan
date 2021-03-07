using System;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter.Exporter
{
    internal interface IVertexMapper<T> where T : unmanaged
    {
        Mesh<T> Map(ReadOnlySpan<ObjVertex> vertices, ReadOnlyMemory<int> indices, ReadOnlyMemory<SubMesh> submeshes);
    }
}

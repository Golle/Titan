using System;
using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter
{
    internal interface IVertexMapper<T> where T : unmanaged
    {
        Mesh<T> Map(ReadOnlySpan<ObjVertex> vertices, ReadOnlySpan<int> indices, ReadOnlySpan<SubMesh> submeshes);
    }
}

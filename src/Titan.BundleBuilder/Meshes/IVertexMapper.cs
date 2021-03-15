using System;
using Titan.BundleBuilder.WavefrontObj;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.BundleBuilder.Meshes
{
    internal interface IVertexMapper<T> where T : unmanaged
    {
        Mesh<T> Map(ReadOnlySpan<ObjVertex> vertices, ReadOnlyMemory<int> indices, ReadOnlyMemory<SubMeshData> submeshes);
    }
}

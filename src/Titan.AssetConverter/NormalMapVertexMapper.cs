using System;
using System.Numerics;
using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter
{
    internal class NormalMapVertexMapper : IVertexMapper<VertexTangentBiNormal>
    {
        private readonly WavefrontObject _model;

        public NormalMapVertexMapper(WavefrontObject model)
        {
            _model = model;
        }

        public Mesh<VertexTangentBiNormal> Map(ReadOnlySpan<ObjVertex> vertices, ReadOnlySpan<int> indices, ReadOnlySpan<SubMesh> submeshes)
        {
            var v = new VertexTangentBiNormal[vertices.Length];
            for (var i = 0; i < vertices.Length; ++i)
            {
                ref readonly var vertex = ref vertices[i];
                ref var targetVertex = ref v[i];

                // .obj file is RightHanded, the engine only supports LeftHanded coordinates so we flip position and normal Z coordinate
                ref var position = ref _model.Positions[vertex.VertexIndex];
                targetVertex.Position = new Vector3(position.X, position.Y, -position.Z);

                if (vertex.TextureIndex != -1)
                {
                    ref readonly var texture = ref _model.Textures[vertex.TextureIndex];
                    targetVertex.Texture = new Vector2(texture.X, 1f - texture.Y);
                }

                if (vertex.NormalIndex != -1)
                {
                    ref readonly var normal = ref _model.Normals[vertex.NormalIndex];
                    targetVertex.Normal = new Vector3(normal.X, normal.Y, -normal.Z);
                }
            }

            //TODO: Add bounding box when we need it
            return new Mesh<VertexTangentBiNormal>(v, indices.ToArray(), submeshes.ToArray());
        }
    }
}

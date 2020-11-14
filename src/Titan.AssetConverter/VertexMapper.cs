using System;
using System.Numerics;
using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter
{
    internal class VertexMapper : IVertexMapper<Vertex>
    {
        private readonly WavefrontObject _model;

        public VertexMapper(WavefrontObject model)
        {
            _model = model;
        }
        public Mesh<Vertex> Map(ReadOnlySpan<ObjVertex> vertices, ReadOnlySpan<int> indices, ReadOnlySpan<SubMesh> submeshes)
        {
            var v = new Vertex[vertices.Length];
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

            //TODO: Add bouding box when we need it
            return new Mesh<Vertex>(v, indices.ToArray(), submeshes.ToArray());
        }

        

        //        // TODO: fix this when everything else works
        //        //for (var i = 0; i < _submeshCount; ++i)
        //        //{
        //        //    ref var submesh = ref _meshes[i];
        //        //    SetBoundingBox(ref submesh, new Span<int>(_indices, submesh.StartIndex, submesh.Count), vertices);
        //        //}
        //    }
        //    return new Mesh<T>(result, new Span<int>(_indices, 0, _indexCount).ToArray(), new Span<SubMesh>(_meshes, 0, _submeshCount).ToArray());
        //}

        //[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        //private static void SetBoundingBox(ref SubMesh mesh, Span<int> indices, in T[] vertices)
        //{
        //    var min = new Vector3(float.MaxValue);
        //    var max = new Vector3(float.MinValue);
        //    foreach (var index in indices)
        //    {
        //        ref var position = ref vertices[index].Position;
        //        if (position.X < min.X) min.X = position.X;
        //        if (position.X > max.X) max.X = position.X;
        //        if (position.Y < min.Y) min.Y = position.Y;
        //        if (position.Y > max.Y) max.Y = position.Y;
        //        if (position.Z < min.Z) min.Z = position.Z;
        //        if (position.Z > max.Z) max.Z = position.Z;
        //    }

        //    mesh.Min = min;
        //    mesh.Max = max;
        //}

    }
}

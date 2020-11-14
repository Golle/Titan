using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter.Exporter
{
    internal class MeshBuilder
    {
        private readonly WavefrontObject _obj;

        private readonly ObjVertex[] _vertices = new ObjVertex[200_000];
        private readonly int[] _indices = new int[800_000];
        private readonly SubMesh[] _meshes = new SubMesh[10000];

        private int _indexCount;
        private int _vertexCount;
        private int _submeshCount;

        private int _currentMaterial = -1;

        public MeshBuilder(WavefrontObject obj)
        {
            _obj = obj;
        }

        public void AddVertex(in ObjVertex objVertex)
        {
            var vertexIndex = -1;
            for (var i = 0; i < _vertexCount; ++i)
            {
                ref var a = ref _vertices[i];
                if (a.NormalIndex == objVertex.NormalIndex && a.TextureIndex == objVertex.TextureIndex && objVertex.VertexIndex == a.VertexIndex)
                {
                    vertexIndex = i;
                    break;
                }
            }

            if (vertexIndex != -1)
            {
                _indices[_indexCount++] = vertexIndex;
            }
            else
            {
                _indices[_indexCount++] = _vertexCount;
                _vertices[_vertexCount++] = objVertex;
            }
        }

        public void SetMaterial(in int material)
        {
            if (_currentMaterial == material)
            {
                return;
            }
            //Console.WriteLine($"[{material}] = @\"{_obj.Materials[material].DiffuseMap}\"");
            _currentMaterial = material;
            SetCountForCurrentMesh();
            ref var mesh = ref _meshes[_submeshCount++];
            mesh.StartIndex = _indexCount;
            mesh.MaterialIndex = _currentMaterial;
        }

        private void SetCountForCurrentMesh()
        {
            if (_submeshCount > 0)
            {
                ref var mesh = ref _meshes[_submeshCount - 1];
                mesh.Count = _indexCount - mesh.StartIndex;
            }
        }

        public Mesh Build()
        {
            SetCountForCurrentMesh();

            var vertices = new Vertex[_vertexCount];
            for (var i = 0; i < _vertexCount; ++i)
            {
                ref var vertex = ref _vertices[i];
                ref var position = ref _obj.Positions[vertex.VertexIndex];

                // .obj file is RightHanded, the engine only supports LeftHanded coordinates so we flip position and normal Z coordinate
                vertices[i] = new Vertex
                {
                    Position = new Vector3(position.X, position.Y, -position.Z)
                };
                if (vertex.TextureIndex != -1)
                {
                    ref var texture = ref _obj.Textures[vertex.TextureIndex];
                    vertices[i].Texture = new Vector2(texture.X, 1f - texture.Y);// TODO: not sure about this one
                }
                if (vertex.NormalIndex != -1)
                {
                    ref var normal = ref _obj.Normals[vertex.NormalIndex];
                    vertices[i].Normal = new Vector3(normal.X, normal.Y, -normal.Z);
                }
            }

            for (var i = 0; i < _submeshCount; ++i)
            {
                ref var submesh = ref _meshes[i];
                SetBoundingBox(ref submesh, new Span<int>(_indices, submesh.StartIndex, submesh.Count), vertices);
            }

            return new Mesh(vertices, new Span<int>(_indices, 0, _indexCount).ToArray(), new Span<SubMesh>(_meshes, 0, _submeshCount).ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        private static void SetBoundingBox(ref SubMesh mesh, Span<int> indices, in Vertex[] vertices)
        {
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var index in indices)
            {
                ref var position = ref vertices[index].Position;
                if (position.X < min.X) min.X = position.X;
                if (position.X > max.X) max.X = position.X;
                if (position.Y < min.Y) min.Y = position.Y;
                if (position.Y > max.Y) max.Y = position.Y;
                if (position.Z < min.Z) min.Z = position.Z;
                if (position.Z > max.Z) max.Z = position.Z;
            }

            mesh.Min = min;
            mesh.Max = max;
        }
    }
}

using System;
using Titan.BundleBuilder.WavefrontObj;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.BundleBuilder.Meshes
{
    internal class MeshBuilder
    {
        private readonly ObjVertex[] _vertices = new ObjVertex[200_000];
        private readonly int[] _indices = new int[800_000];
        private readonly SubMeshData[] _meshes = new SubMeshData[10000];

        private int _indexCount;
        private int _vertexCount;
        private int _submeshCount;

        private int _currentMaterial = -1;

        public void AddVertex(in ObjVertex objVertex)
        {
            var vertexIndex = -1;
            for (var i = 0; i < _vertexCount; ++i)
            {
                if (_vertices[i] == objVertex)
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
            _currentMaterial = material;
            SetCountForCurrentMesh();
            ref var mesh = ref _meshes[_submeshCount++];
            mesh.StartIndex = (uint) _indexCount;
            mesh.MaterialIndex = _currentMaterial;
        }

        private void SetCountForCurrentMesh()
        {
            if (_submeshCount > 0)
            {
                ref var mesh = ref _meshes[_submeshCount - 1];
                mesh.Count = (uint) (_indexCount - mesh.StartIndex);
            }
        }

        public Mesh<T> Build<T>(IVertexMapper<T> mapper) where T : unmanaged
        {
            SetCountForCurrentMesh();
            return mapper.Map(new ReadOnlySpan<ObjVertex>(_vertices, 0, _vertexCount), new ReadOnlyMemory<int>(_indices, 0, _indexCount), new ReadOnlyMemory<SubMeshData>(_meshes, 0, _submeshCount));
        }
    }
}

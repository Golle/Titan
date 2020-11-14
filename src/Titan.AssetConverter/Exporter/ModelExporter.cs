using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Titan.AssetConverter.Files;

namespace Titan.AssetConverter.Exporter
{
    internal class ModelExporter
    {
        private readonly IByteWriter _byteWriter = new ByteWriter();
        
        // TODO: make this more dynamic in the future.
        public async Task ExportModel(Mesh<Vertex> mesh, Mesh<VertexTangentBiNormal> normalMapMesh, string filename)
        {
            await using var file = File.OpenWrite(filename);
            file.SetLength(0);
            var numberOfChunks = (ushort) (normalMapMesh.Vertices.Length > 0 ? 2 : 1); // Temp solution
            unsafe
            {
                var header = new Header
                {
                    Version = 1,
                    NumberOfChunks = numberOfChunks,
                    //VertexSize = sizeof(T),
                    //VertexCount = mesh.Vertices.Length,
                    //IndexSize = sizeof(int),
                    //IndexCount = mesh.Indices.Length,
                    //SubMeshCount = mesh.SubMeshes.Length,
                };
                Console.WriteLine($"Writing header to '{filename}' with number of chunks = {numberOfChunks}");
                _byteWriter.Write(file, header);
            }


            Console.WriteLine($"Writing chunk 1 header");
            unsafe
            {
                var chunkHeader = new ChunkHeader { IndexCount = mesh.Indices.Length, IndexSize = sizeof(int), SubMeshCount = mesh.SubMeshes.Length, VertexCount = mesh.Vertices.Length, VertexSize = sizeof(Vertex) };
                _byteWriter.Write(file, chunkHeader);
            }

            Console.WriteLine($"Writing chunk 1 with submeshes {mesh.SubMeshes.Length}");
            _byteWriter.Write(file, mesh.SubMeshes);
            Console.WriteLine($"Writing chunk 1 with vertices {mesh.Vertices.Length}");
            _byteWriter.Write(file, mesh.Vertices);
            Console.WriteLine($"Writing chunk 1 with indices {mesh.Indices.Length}");
            _byteWriter.Write(file, mesh.Indices);

            if (numberOfChunks > 1)
            {
                Console.WriteLine($"Writing chunk 2 header");
                unsafe
                {
                    var chunkHeader = new ChunkHeader { IndexCount = normalMapMesh.Indices.Length, IndexSize = sizeof(int), SubMeshCount = normalMapMesh.SubMeshes.Length, VertexCount = normalMapMesh.Vertices.Length, VertexSize = sizeof(VertexTangentBiNormal) };
                    _byteWriter.Write(file, chunkHeader);
                }
                Console.WriteLine($"Writing chunk 2 with submeshes {normalMapMesh.SubMeshes.Length}");
                _byteWriter.Write(file, normalMapMesh.SubMeshes);
                Console.WriteLine($"Writing chunk 2 with vertices {normalMapMesh.Vertices.Length}");
                _byteWriter.Write(file, normalMapMesh.Vertices);
                Console.WriteLine($"Writing chunk 2 with indices {normalMapMesh.Indices.Length}");
                _byteWriter.Write(file, normalMapMesh.Indices);
            }
        }

        public async Task ExportMaterials(Material[] materials, string filename)
        {
            Console.WriteLine($"Writing {materials.Length} materials to {filename}");
            await using var file = File.OpenWrite(filename);
            file.SetLength(0); // Reset the file
            await JsonSerializer.SerializeAsync(file, materials, new JsonSerializerOptions{WriteIndented = true});
            await file.FlushAsync();
        }
    }
}

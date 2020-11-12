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
        
        public async Task ExportModel(Mesh mesh, string filename)
        {
            await using var file = File.OpenWrite(filename);
            file.SetLength(0);
            unsafe
            {
                var header = new Header
                {
                    VertexSize = sizeof(Vertex),
                    VertexCount = mesh.Vertices.Length,
                    IndexSize = sizeof(int),
                    IndexCount = mesh.Indices.Length,
                    SubMeshCount = mesh.SubMeshes.Length,
                };
                Console.WriteLine($"Writing header to '{filename}' with mesh count {header.SubMeshCount}");
                _byteWriter.Write(file, header);
            }
            Console.WriteLine($"Writing submesh {mesh.SubMeshes.Length}");
            _byteWriter.Write(file, mesh.SubMeshes);
            Console.WriteLine($"Writing vertices {mesh.Vertices.Length}");
            _byteWriter.Write(file, mesh.Vertices);
            Console.WriteLine($"Writing indices {mesh.Indices.Length}");
            _byteWriter.Write(file, mesh.Indices);
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

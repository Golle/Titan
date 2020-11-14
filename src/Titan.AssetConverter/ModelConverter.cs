using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter
{
    public record Material(string Name, string Diffuse, string DiffuseMap, string Ambient, string Specular, string Emissive, string NormalMap, bool IsTextured, bool IsTransparent);

    internal class ModelConverter
    {
        private readonly ObjParser _parser = new();
        private static readonly ModelExporter _exporter = new();

        public async Task Convert(string objFile, string outputModel, string outputMaterial)
        {
            var timer = Stopwatch.StartNew();
            var model = await _parser.ReadFromFile(objFile);

            var meshTask = ExportMesh(model, outputModel);
            var materialTask = model.Materials != null ? _exporter.ExportMaterials(model.Materials.Select(ConvertMaterial).ToArray(), outputMaterial) : Task.CompletedTask;

            await Task.WhenAll(meshTask, materialTask);

            timer.Stop();
            Console.WriteLine($"Finished {Path.GetFileName(objFile)} in {timer.Elapsed.TotalMilliseconds:## 'ms'}");
        }

        private static Material ConvertMaterial(ObjMaterial material)
        {
            return new(
                material.Name,
                material.DiffuseColor.ToString(),
                material.DiffuseMap,
                material.AmbientColor.ToString(),
                material.SpecularColor.ToString(),
                material.EmissiveColor.ToString(),
                material.BumpMap,
                !string.IsNullOrWhiteSpace(material.DiffuseMap),
                material.Transparency != 0f // TODO: this is not a good way to solve this. 
            );
        }

        private static async Task ExportMesh(WavefrontObject model, string filename)
        {
            var normalMapBuilder = new MeshBuilder();
            var vertexBuilder = new MeshBuilder();

            foreach (var objGroup in model.Groups)
            {
                foreach (var face in objGroup.Faces)
                {
                    var builder = model.Materials?[face.Material].BumpMap == null ? vertexBuilder : normalMapBuilder;

                    builder.SetMaterial(face.Material);
                    //// TODO: add support for Concave faces (triangles done this way might overlap)
                    //// RH
                    //// 1st face => vertex 0, 1, 2
                    //// 2nd face => vertex 0, 2, 3
                    //// 3rd face => vertex 0, 4, 5
                    //// 4th face => ...

                    var vertices = face.Vertices;
                    // more than 3 vertices per face, we need to triangulate the face to be able to use it in the engine.
                    for (var i = 2; i < vertices.Length; ++i)
                    {
                        builder.AddVertex(vertices[0]);
                        //Flip the order to convert from RH to LH (d3d) 
                        builder.AddVertex(vertices[i]);
                        builder.AddVertex(vertices[i - 1]);
                    }
                }
            }


            var normalMapMesh = normalMapBuilder.Build(new NormalMapVertexMapper(model));
            var mesh = vertexBuilder.Build(new VertexMapper(model));

            await _exporter.ExportModel(mesh, normalMapMesh, filename);
        }
    }
}
    

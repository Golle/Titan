using System;
using System.Threading.Tasks;
using Titan.AssetConverter.Exporter;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter
{
    internal class ModelConverter
    {
        private readonly ObjParser _parser = new ObjParser();
        private readonly ModelExporter _exporter = new ModelExporter();
        public async Task Convert(string objFile, string outputModel, string outputMaterial)
        {
            var model = await _parser.ReadFromFile(objFile);
            var mesh = CreateMesh(model);
            var material = CreateMaterial(model);
            
            await _exporter.ExportModel(mesh, outputModel);

            Console.WriteLine("Finished");
        }

        private object CreateMaterial(WavefrontObject model)
        {
            return null;
        }

        private static Mesh CreateMesh(WavefrontObject model)
        {
            var builder = new MeshBuilder(model);
            foreach (var objGroup in model.Groups)
            {
                foreach (var face in objGroup.Faces)
                {
                    builder.SetMaterial(face.Material);
                    // TODO: add support for Concave faces (triangles done this way might overlap)
                    // RH
                    // 1st face => vertex 0, 1, 2
                    // 2nd face => vertex 0, 2, 3
                    // 3rd face => vertex 0, 4, 5
                    // 4th face => ...

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

            return builder.Build();
        }
    }
}

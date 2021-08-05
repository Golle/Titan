using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Titan.Core.Logging;
using Titan.Core.Serialization;
using Titan.Graphics;
using Titan.Graphics.Loaders.Materials;
using Titan.Graphics.Loaders.Models;
using Tools.Core.WavefrontObj;
using Tools.ModelBuilder;

Logger.Start();

Logger.Info("Hello world!");


Console.WriteLine(Directory.GetCurrentDirectory());


var assetsPath = @"F:\Git\Titan\assets";
var modelDestinationPath = @"F:\Git\Titan\assetsV2\models";
var materialDestinationPath = @"F:\Git\Titan\assetsV2\materials";

if (args.Length == 3)
{
    static string GetPath(string path) => Path.IsPathRooted(path) ? path : Path.Combine(Directory.GetCurrentDirectory(), path);

    assetsPath = GetPath(args[0]);
    modelDestinationPath = GetPath(args[1]);
    materialDestinationPath = GetPath(args[2]);
}
else
{
    Logger.Warning("No arguments specified, using default.");
}

Directory.CreateDirectory(modelDestinationPath);
Directory.CreateDirectory(materialDestinationPath);

foreach (var file in Directory.EnumerateFiles(assetsPath, "*.obj", SearchOption.AllDirectories))
{
    var timer = Stopwatch.StartNew();
    var model = await ObjParser.ReadFromFile(file);
    var name = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
    Logger.Info($"Found '{name}'");

    var builder = new MeshBuilder();
    foreach (var group in model.Groups)
    {
        foreach (var face in group.Faces)
        {
            // A new material indicates a new "sub mesh". This is where we create a new mesh. (if it's the first one we don't do anything except setting the material
            builder.SetMaterial(face.Material);

            ////// TODO: add support for Concave faces (triangles done this way might overlap)
            ////// RH
            ////// 1st face => vertex 0, 1, 2
            ////// 2nd face => vertex 0, 2, 3
            ////// 3rd face => vertex 0, 4, 5
            ////// 4th face => ...
            var faceVertices = face.Vertices;
            //// more than 3 vertices per face, we need to triangulate the face to be able to use it in the engine.
            for (var i = 2; i < faceVertices.Length; ++i)
            {
                builder.AddVertex(faceVertices[0]);
                //Flip the order to convert from RH to LH (d3d) 
                builder.AddVertex(faceVertices[i]);
                builder.AddVertex(faceVertices[i - 1]);
            }
        }
    }

    var mesh = builder.Build((objVertices, objIndices, submeshes) =>
    {
        var vertices = new TexturedVertex[objVertices.Length];
        var objVerticesSpan = objVertices.Span;
        for (var i = 0; i < objVertices.Length; ++i)
        {
            ref readonly var vertex = ref objVerticesSpan[i];
            ref var targetVertex = ref vertices[i];

            // .obj file is RightHanded, the engine only supports LeftHanded coordinates so we flip position and normal Z coordinate
            ref var position = ref model.Positions[vertex.VertexIndex];
            targetVertex.Position = new Vector3(position.X, position.Y, -position.Z);

            if (vertex.TextureIndex != -1)
            {
                ref readonly var texture = ref model.Textures[vertex.TextureIndex];
                targetVertex.Texture = new Vector2(texture.X, 1f - texture.Y);
            }

            if (vertex.NormalIndex != -1)
            {
                ref readonly var normal = ref model.Normals[vertex.NormalIndex];
                targetVertex.Normal = new Vector3(normal.X, normal.Y, -normal.Z);
            }
        }
        return new Mesh<TexturedVertex>(vertices, objIndices, submeshes);
    });


    await using var modelOutput = File.Open(Path.Combine(modelDestinationPath, $"{name}.geo"), FileMode.OpenOrCreate, FileAccess.Write);
    modelOutput.Seek(0, SeekOrigin.Begin);
    modelOutput.SetLength(0);
    unsafe
    {
        var desc = new MeshDescriptor
        {
            VertexSize = (uint) sizeof(TexturedVertex),
            NumberOfIndices = (uint) mesh.Indices.Length,
            NumberOfSubmeshes = mesh.SubMeshes.Length,
            NumberOfVertices = (uint) mesh.Vertices.Length,
        };
        modelOutput.Write(new ReadOnlySpan<byte>(&desc, sizeof(MeshDescriptor)));

        fixed (TexturedVertex* pVertices = mesh.Vertices.Span)
        {
            modelOutput.Write(new ReadOnlySpan<byte>(pVertices, sizeof(TexturedVertex) * mesh.Vertices.Length));
        }
        
        fixed (int* pIndicies = mesh.Indices.Span)
        {
            modelOutput.Write(new ReadOnlySpan<byte>(pIndicies, sizeof(int) * mesh.Indices.Length));
        }

        fixed (SubmeshDescriptor* pSubmeshes = mesh.SubMeshes.Span)
        {
            modelOutput.Write(new ReadOnlySpan<byte>(pSubmeshes, sizeof(SubmeshDescriptor) * mesh.SubMeshes.Length));
        }
    }


    {

        for (var i = 0; i < model.Materials.Length; ++i)
        {
            var material = model.Materials[i];

            await using var materialOutput = File.Open(Path.Combine(materialDestinationPath, $"{name}_{i:D2}.json"), FileMode.OpenOrCreate, FileAccess.Write);
            materialOutput.SetLength(0);
            materialOutput.Seek(0, SeekOrigin.Begin);
            materialOutput.Write(Json.SerializeUtf8(new MaterialDescriptor
            {
                Name = material.Name,
                DiffuseColor = Color.ParseF(material.DiffuseColor.Original, Color.White),
                AmbientColor = Color.ParseF(material.AmbientColor.Original, Color.White),
                EmissiveColor = Color.ParseF(material.EmissiveColor.Original, Color.Zero),
                SpecularColor = Color.ParseF(material.SpecularColor.Original, Color.Zero)
            }));
        }
    }

    Logger.Info($"Converted '{name}' in {timer.Elapsed.TotalMilliseconds} ms.");
}
Logger.Info("Done");
Logger.Shutdown();




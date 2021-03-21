using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Titan.BundleBuilder.Common;
using Titan.GraphicsV2.Resources.Materials;
using Titan.GraphicsV2.Resources.Models;

namespace Titan.BundleBuilder.Bundles
{
    internal class CreateDataChunks : IMiddleware<BundleContext>
    {
        public async Task<BundleContext> Invoke(BundleContext context, MiddlewareDelegate<BundleContext> next)
        {
            List<TextureDescriptor> textures = new();
            List<MaterialDescriptor> materials = new();
            List<MeshDescriptor> meshes = new();

            var stream = new MemoryStream();
            using var writer = new ByteStreamWriter(stream);
            
            foreach (var model in context.Models)
            {
                var offset = stream.Position;
                var mesh = model.Mesh;
                writer.Write(mesh.Vertices);
                writer.Write(mesh.Indices);
                writer.Write(mesh.SubMeshes);
                
                unsafe
                {
                    meshes.Add(new MeshDescriptor
                    {
                        Name = model.ModelSpecification.Name,
                        Offset = offset,
                        IndexCount = mesh.Indices.Length,
                        IndexSize = sizeof(int),
                        VertexCount = mesh.Vertices.Length,
                        VertexSize = sizeof(VertexData),
                        SubmeshCount = mesh.SubMeshes.Length,
                        SubmeshSize = sizeof(SubMeshData)
                    });
                }
            }

            foreach (var texture in context.Textures)
            {
                //var image = texture.Image;
                var offset = stream.Position;
                writer.Write(new ReadOnlyMemory<byte>(texture.Data));
                textures.Add(new TextureDescriptor
                {
                    Name = texture.TextureSpecification.Name,
                    Filename = texture.TextureSpecification.Filename,
                    Offset = offset,
                    //Format = image.Format,
                    //Height = image.Height,
                    //Width = image.Width,
                    //Size = image.ImageSize,
                    //Stride = image.Stride
                });
            }

            int GetTextureIndex(string path) => string.IsNullOrWhiteSpace(path) ? -1 : textures.FindIndex(descriptor => descriptor.Filename == path);

            foreach (var model in context.Models)
            {
                var offset = stream.Position;
                foreach (var material in model.Materials)
                {
                    var data = new MaterialData
                    {
                        Ambient = material.Ambient,
                        Diffuse = material.Diffuse,
                        Emissive = material.Emissive,
                        Specular = material.Specular,
                        DiffuseTextureIndex = GetTextureIndex(material.DiffuseMapPath),
                        NormalTextureIndex = GetTextureIndex(material.NormalMapPath)
                    };
                    writer.Write(data);
                }
                materials.Add(new MaterialDescriptor
                {
                    Count = model.Materials.Length,
                    Offset = offset
                });
            }
            
            await writer.FlushAsync();

            return await next(context with
            {
                DataBlob = stream.ToArray(),
                MeshDescriptors = meshes.ToArray(),
                TextureDescriptors = textures.ToArray(),
                MaterialDescriptors = materials.ToArray(),
            });

        }
    }
}

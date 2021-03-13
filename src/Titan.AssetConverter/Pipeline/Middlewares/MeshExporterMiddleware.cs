using System;
using System.IO;
using System.Threading.Tasks;
using Titan.AssetConverter.Common;
using Titan.GraphicsV2.Resources;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal class MeshExporterMiddleware : IMiddleware<MeshContext>
    {
        public async Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            context = await next(context);

            var mesh = context.Mesh;
            if (mesh == null)
            {
                throw new InvalidOperationException("No mesh found");
            }

            var path = Path.Combine(context.OutputFolder, $"{context.Name}.dat");
            await using var file = File.OpenWrite(path);
            file.Seek(0, SeekOrigin.Begin);
            file.SetLength(0);

            var writer = new ByteStreamWriter(file);
            unsafe
            {
                writer.Write(new MeshDataHeader
                {
                    Vertices = mesh.Vertices.Length,
                    VertexSize = sizeof(VertexData),
                    Indicies = mesh.Indices.Length,
                    IndexSize = sizeof(int),
                    SubMeshes = mesh.SubMeshes.Length,
                    SubMeshSize = sizeof(SubMeshData)
                });
            }
            
            writer.Write(mesh.Vertices);
            writer.Write(mesh.Indices);
            writer.Write(mesh.SubMeshes);

            Logger.Info($"{context.Name}, submesh count: {context.Mesh.SubMeshes.Length}");

            return context;
        }

        private struct MeshDataHeader
        {
            internal int Indicies;
            internal int IndexSize;
            internal int Vertices;
            internal int VertexSize;
            internal int SubMeshes;
            internal int SubMeshSize;
        }
    }
}

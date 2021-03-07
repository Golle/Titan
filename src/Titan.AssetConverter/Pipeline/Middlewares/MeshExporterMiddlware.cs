using System;
using System.Threading.Tasks;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal class MeshExporterMiddlware : IMiddleware<MeshContext>
    {
        public async Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            context = await next(context);

            if (context.Mesh == null)
            {
                throw new InvalidOperationException("No mesh found");
            }

            Logger.Info($"{context.Name}, submesh count: {context.Mesh.SubMeshes.Length}");

            return context;
        }
    }
}

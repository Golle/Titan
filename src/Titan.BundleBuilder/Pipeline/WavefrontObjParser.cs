using System.IO;
using System.Threading.Tasks;
using Titan.BundleBuilder.WavefrontObj;

namespace Titan.BundleBuilder.Pipeline
{
    internal class WavefrontObjParser : IMiddleware<ModelContext>
    {
        public async Task<ModelContext> Invoke(ModelContext context, MiddlewareDelegate<ModelContext> next)
        {
            var meshPath = Path.Combine(context.AssetsPath, "models", context.ModelDescriptor.Filename);
            var obj = await ObjParser.ReadFromFile(meshPath);
            return await next(context with { Object = obj });
        }
    }
}

using System.IO;
using System.Threading.Tasks;
using Titan.AssetConverter.WavefrontObj;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal class WavefronObjectReaderMiddleware : IMiddleware<MeshContext>
    {
        public async Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            var result = await ObjParser.ReadFromFile(context.Filename);

            return await next(context with {Object = result, Name = Path.GetFileNameWithoutExtension(context.Filename)});
        }
    }
}

using System.Threading.Tasks;
using Titan.BundleBuilder.Common;

namespace Titan.BundleBuilder.Textures
{
    internal class LoadAndConvertTexture : IMiddleware<TextureContext>
    {
        public async Task<TextureContext> Invoke(TextureContext context, MiddlewareDelegate<TextureContext> next)
        {
            Logger.Info(context.TextureSpecification.Name);
            Logger.Info(context.TextureSpecification.Filename);

            using var loader = new ImageLoader();
            var image = loader.Load(context.TextureSpecification.Filename);
            return await next(context with { Image = image });
        }
       
    }
}

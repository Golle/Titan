using System.IO;
using System.Threading.Tasks;

namespace Titan.BundleBuilder.Textures
{
    internal class LoadTexture : IMiddleware<TextureContext>
    {
        public async Task<TextureContext> Invoke(TextureContext context, MiddlewareDelegate<TextureContext> next)
        {
            Logger.Info(context.TextureSpecification.Name);
            Logger.Info(context.TextureSpecification.Filename);

            // TODO: use this line if we want to format the texture to RGBA32 before writing it into the bundle
            // The samples I used increased the size from 2.5mb to 11.5mb when doing this.

            //using var loader = new ImageLoader();
            //var image = loader.Load(context.TextureSpecification.Filename);

            var bytes = await File.ReadAllBytesAsync(context.TextureSpecification.Filename);
            //var image = new Image(bytes, (uint) bytes.Length, DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, 0, 0, 0);

            return await next(context with { Data = bytes });
        }
       
    }
}

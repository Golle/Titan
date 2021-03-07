using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal class MaterialExporterMiddleware : IMiddleware<MeshContext>
    {
        public async Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            context = await next(context);
            if (context.Materials.Length == 0)
            {
                Logger.Info("No Materials found.");
                return context;
            }
            Logger.Info($"{context.Name}, material count: {context.Materials.Length}");

            var path = Path.Combine(context.OutputFolder, $"{context.Name.ToLower()}.json");
            await using var file = File.OpenWrite(path);
            file.Seek(0, SeekOrigin.Begin); // Reset the file
            file.SetLength(0);

            await JsonSerializer.SerializeAsync(file, context.Materials, new JsonSerializerOptions {WriteIndented = true});
            await file.FlushAsync();
            
            return context;
        }
    }
}

using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Titan.Windows.Win32.D3D11;

namespace Titan.BundleBuilder.Bundles
{
    internal class WriteBundle : IMiddleware<BundleContext>
    {
        public async Task<BundleContext> Invoke(BundleContext context, MiddlewareDelegate<BundleContext> next)
        {

            Logger.Info($"Writing bundle {context.Name}");
            foreach (var model in context.Models)
            {
                Logger.Info($"Write model {model.ModelSpecification.Name}");
            }
            foreach (var model in context.Textures)
            {
                Logger.Info($"Write texture {model.TextureSpecification.Name}");
            }
            return await next(context);
        }
    }


    internal record TextureDescriptor
    {
        internal string Name { get; init; }
        internal long Offset { get; init; }
        internal uint Height { get; init; }
        internal uint Width { get; init; }
        internal uint Stride { get; init; }
        internal uint Size { get; init; }
        internal DXGI_FORMAT Format { get; init; } // TODO: maybe use some format that is cross platform?
        
        [JsonIgnore]
        public string Filename { get; init; }
    }

    internal record MeshDescriptor
    {
        internal string Name { get; init; }
        internal long Offset { get; init; }
        internal int VertexSize { get; init; }
        internal int VertexCount { get; init; }
        internal int IndexSize { get; init; }
        internal int IndexCount { get; init; }
        internal int SubmeshSize { get; init; }
        internal int SubmeshCount { get; init; }
    }

    internal record MaterialDescriptor
    {
        internal long Offset { get; init; }
        internal int Count { get; init; }
    }
}

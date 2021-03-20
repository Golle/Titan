using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Titan.Windows.Win32.D3D11;

namespace Titan.BundleBuilder.Bundles
{

    internal record BundleDescriptor
    {
        public MeshDescriptor[] Meshes { get; init; }
        public TextureDescriptor[] Textures { get; init; }
        public MaterialDescriptor[] Materials { get; init; }
    }
    internal class WriteBundle : IMiddleware<BundleContext>
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };

        public async Task<BundleContext> Invoke(BundleContext context, MiddlewareDelegate<BundleContext> next)
        {
            var json = JsonSerializer.Serialize(new BundleDescriptor
            {
                Materials = context.MaterialDescriptors,
                Textures = context.TextureDescriptors,
                Meshes = context.MeshDescriptors
            }, Options);
            await File.WriteAllTextAsync(Path.Combine(Configuration.OutputPath, $"{context.Name}.json"), json);
            await File.WriteAllBytesAsync(Path.Combine(Configuration.OutputPath, $"{context.Name}.dat"), context.DataBlob);

            return await next(context);
        }
    }


    internal record TextureDescriptor
    {
        public string Name { get; init; }
        public long Offset { get; init; }
        public uint Height { get; init; }
        public uint Width { get; init; }
        public uint Stride { get; init; }
        public uint Size { get; init; }
        public DXGI_FORMAT Format { get; init; } // TODO: maybe use some format that is cross platform?
        
        
        internal string Filename { get; init; }
    }

    internal record MeshDescriptor
    {
        public string Name { get; init; }
        public long Offset { get; init; }
        public int VertexSize { get; init; }
        public int VertexCount { get; init; }
        public int IndexSize { get; init; }
        public int IndexCount { get; init; }
        public int SubmeshSize { get; init; }
        public int SubmeshCount { get; init; }
    }

    internal record MaterialDescriptor
    {
        public long Offset { get; init; }
        public int Count { get; init; }
    }
}

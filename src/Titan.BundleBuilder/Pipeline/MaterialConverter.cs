using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Titan.BundleBuilder.Pipeline
{
    internal record Material
    {
        public string Name { get; init; }
        public string DiffuseMapPath { get; init; }
        public string NormalMapPath { get; init; }
    }

    internal class MaterialConverter : IMiddleware<ModelContext>
    {
        public async Task<ModelContext> Invoke(ModelContext context, MiddlewareDelegate<ModelContext> next)
        {
            var modelPath = Path.GetDirectoryName(Path.Combine(context.AssetsPath, "models", context.ModelDescriptor.Filename)) ?? throw new InvalidOperationException("Failed to get the path to the model");
            
            string GetPath(string name) => string.IsNullOrWhiteSpace(name) ? null : Path.Combine(modelPath, name);

            var materials = context
                .Object
                .Materials
                .Select(m => new Material
                {
                    Name = m.Name,
                    DiffuseMapPath = GetPath(m.DiffuseMap),
                    NormalMapPath = GetPath(m.BumpMap)
                })
                .ToArray();

            return await next(context with { Materials = materials});
        }
    }
}

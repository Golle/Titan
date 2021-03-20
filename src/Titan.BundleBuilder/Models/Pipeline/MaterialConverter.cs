using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Titan.BundleBuilder.WavefrontObj;
using Titan.GraphicsV2.D3D11;

namespace Titan.BundleBuilder.Models.Pipeline
{
    internal record Material
    {
        public string Name { get; init; }
        public string DiffuseMapPath { get; init; }
        public string NormalMapPath { get; init; }
        public Color Ambient { get; init; }
        public Color Diffuse { get; init; }
        public Color Emissive { get; init; }
        public Color Specular { get; init; }
    }

    internal class MaterialConverter : IMiddleware<ModelContext>
    {
        public async Task<ModelContext> Invoke(ModelContext context, MiddlewareDelegate<ModelContext> next)
        {
            var modelPath = Path.GetDirectoryName(Path.Combine(Configuration.ModelsPath, context.ModelSpecification.Filename)) ?? throw new InvalidOperationException("Failed to get the path to the model");
            
            string GetPath(string name) => string.IsNullOrWhiteSpace(name) ? null : Path.GetFullPath(Path.Combine(modelPath, name));
            static Color Parse(in ObjColor color) => color.HasValue ? Color.ParseF(color.Original) : Color.Zero;

            var materials = context
                .Object
                .Materials
                .Select(m => new Material
                {
                    Name = m.Name,
                    DiffuseMapPath = GetPath(m.DiffuseMap),
                    NormalMapPath = GetPath(m.BumpMap),
                    Ambient = Parse(m.AmbientColor),
                    Diffuse = Parse(m.DiffuseColor),
                    Emissive = Parse(m.EmissiveColor),
                    Specular = Parse(m.SpecularColor),
                })
                .ToArray();
            
            return await next(context with { Materials = materials});
        }
    }
}

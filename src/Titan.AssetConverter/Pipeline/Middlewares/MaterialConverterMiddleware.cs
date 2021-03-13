using System;
using System.Linq;
using System.Threading.Tasks;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal record Material(string Name, string Diffuse, string DiffuseMap, string Ambient, string Specular, string Emissive, string NormalMap, bool IsTextured, bool IsTransparent);

    internal class MaterialConverterMiddleware : IMiddleware<MeshContext>
    {
        public async Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            var materials = context.Object
                .Materials
                ?.Select(m => new Material(
                    m.Name,
                    m.DiffuseColor.ToString(),
                    m.DiffuseMap,
                    m.AmbientColor.ToString(),
                    m.SpecularColor.ToString(),
                    m.EmissiveColor.ToString(),
                    m.BumpMap,
                    !string.IsNullOrWhiteSpace(m.DiffuseMap),
                    m.Transparency != 0f // TODO: this is not a good way to solve this. 
                ))
                .ToArray() ?? Array.Empty<Material>();

            return await next(context with {Materials = materials});
        }
    }
}

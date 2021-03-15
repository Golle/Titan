using System;
using System.Linq;
using Titan.Core.Common;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Resources.Textures;

namespace Titan.GraphicsV2.Resources.Materials
{
    internal record MaterialDescriptor(string Name, string Diffuse, string DiffuseMap, string Ambient, string Specular, string Emissive, string NormalMap, bool IsTextured, bool IsTransparent);

    internal class MaterialsLoader
    {
        private readonly TextureLoader _textureLoader;
        private readonly IFileReader _fileReader;
        private readonly IJsonSerializer _jsonSerializer;

        public MaterialsLoader(TextureLoader textureLoader, IFileReader fileReader, IJsonSerializer jsonSerializer)
        {
            _textureLoader = textureLoader;
            _fileReader = fileReader;
            _jsonSerializer = jsonSerializer;
        }

        public Material[] Load(string identifier)
        {
            string content;
            try
            {
                content = _fileReader.ReadText(identifier);
            }
            catch (Exception)
            {
                LOGGER.Trace($"No material found with identifier {identifier}");
                return Array.Empty<Material>();
            }

            var materialDescs = _jsonSerializer.Deserialize<MaterialDescriptor[]>(content);
            var materials = materialDescs.Select(m =>
                {
                    var ambient = string.IsNullOrWhiteSpace(m.Ambient) ? Color.White : Color.ParseF(m.Ambient);
                    var diffuse = string.IsNullOrWhiteSpace(m.Diffuse) ? Color.White : Color.ParseF(m.Diffuse);
                    var diffuseMap = string.IsNullOrWhiteSpace(m.DiffuseMap) ? Handle<Texture>.Null : _textureLoader.Load(m.DiffuseMap);
                    var specular = string.IsNullOrWhiteSpace(m.Specular) ? Color.Zero : Color.ParseF(m.Specular);
                    return new Material
                    {
                        DiffuseTexture = diffuseMap,
                        Properties = new MaterialProperties
                        {
                            Ambient = ambient,
                            Specular = specular,
                            Diffue = diffuse,
                        }
                    };
                })
                .ToArray();
            return materials;
        }
    }
}

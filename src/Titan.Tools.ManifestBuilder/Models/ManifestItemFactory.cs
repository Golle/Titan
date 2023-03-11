using Titan.Tools.Core;
using Titan.Tools.Core.Common;
using Titan.Tools.Core.Manifests;
using Titan.Tools.Core.Shaders;

namespace Titan.Tools.ManifestBuilder.Models;

internal class ManifestItemFactory : IManifestItemFactory
{
    public Result<ManifestItem> CreateFromPath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentNullException(nameof(relativePath));
        }

        var fileExtension = Path
            .GetExtension(relativePath)
            .ToLowerInvariant();

        var name = Path.GetFileNameWithoutExtension(relativePath);

        ManifestItem? item = fileExtension switch
        {
            ".jpg" or ".png" or ".bmp" or ".aseprite" => CreateTexture(relativePath, fileExtension),
            ".obj" or ".fbx" => CreateModel(relativePath),
            ".hlsl" => CreateShader(relativePath),
            ".wav" or ".wave" => CreateAudio(relativePath),
            _ => null
        };

        return item != null ? Result<ManifestItem>.Success(item) : Result<ManifestItem>.Fail($"The file extension {fileExtension} is not supported.");
    }

    private static AudioItem CreateAudio(string relativePath) =>
        new()
        {
            Name = Path.GetFileNameWithoutExtension(relativePath),
            Path = relativePath
        };

    private static ShaderItem CreateShader(string relativePath) =>
        new()
        {
            Name = Path.GetFileNameWithoutExtension(relativePath),
            Path = relativePath,
            ShaderModel = ShaderModels.Unknown,
            EntryPoint = "[CHANGE_ME]"
        };

    private static TextureItem CreateTexture(string path, string fileExtension) =>
        new()
        {
            Name = Path.GetFileNameWithoutExtension(path),
            Path = path,
            Type = fileExtension.ToLowerInvariant() switch
            {
                ".jpg" => TextureType.JPG,
                ".png" => TextureType.PNG,
                ".bmp" => TextureType.BMP,
                ".aseprite" => TextureType.Aseprite,
                _ => TextureType.Unknown
            }
        };

    private static ModelItem CreateModel(string path) =>
        new()
        {
            Name = Path.GetFileNameWithoutExtension(path),
            Path = path
        };
}

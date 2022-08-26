using System;
using System.IO;
using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.Common;

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

        ManifestItem? item = fileExtension switch
        {
            ".jpg" or ".png" or ".bmp" => CreateTexture(relativePath, fileExtension),
            ".obj" or ".fbx" => CreateModel(relativePath),
            _ => null
        };

        return item != null ? Result<ManifestItem>.Success(item) : Result<ManifestItem>.Fail($"The file extension {fileExtension} is not supported.");
    }

    private static TextureItem CreateTexture(string path, string fileExtension) =>
        new()
        {
            Name = Path.GetFileNameWithoutExtension(path),
            Path = path,
            //NOTE(Jens): this wont work if the file extension is upper case.
            Type = fileExtension switch
            {
                ".jpg" => TextureType.JPG,
                ".png" => TextureType.PNG,
                ".bmp" => TextureType.BMP,
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

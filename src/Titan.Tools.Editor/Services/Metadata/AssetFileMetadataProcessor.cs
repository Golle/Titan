using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Titan.Tools.Editor.Configuration;

namespace Titan.Tools.Editor.Services.Metadata;


internal enum TitanAssetType
{
    Unknown,
    Texture,
    Model,
    Shader
}
internal class TitanAssetFileTexture
{
    public int AProperty { get; set; }
}

internal class TitanAssetFileModel
{
    public bool AModelProperty { get; set; }
}

internal class TitanAssetFileShader
{
    public string AShaderProperty { get; set; }
}

internal class TitanAssetFileMetadata
{
    public required TitanAssetType Type { get; init; }
    public TitanAssetFileTexture? Texture { get; set; }
    public TitanAssetFileModel? Model { get; set; }
    public TitanAssetFileShader? Shader { get; set; }
    public string? DisplayName { get; init; }
}


internal class AssetFileMetadataProcessor : IAssetFileProcessor
{
    private const string DeletedFileExtension = ".deleted";

    private static bool IsMetadataFile(string? fileExtension)
        => fileExtension == GlobalConfiguration.TitanAssetMetadataFileExtension || fileExtension == DeletedFileExtension;

    public Task OnFileChanged(AssetFileInfo info)
    {
        Debug.WriteLine($"File changed: {info.Name}");
        return Task.CompletedTask;
    }

    public async Task OnFileCreated(AssetFileCreatedInfo info)
    {
        if (info.IsDirectory || IsMetadataFile(info.FileExtension))
        {
            return;
        }

        var folder = Path.GetDirectoryName(info.FullPath) ?? throw new InvalidOperationException($"Failed to get the folder name from path {info.FullPath}");
        var filename = Path.GetFileNameWithoutExtension(info.Name);
        var metadataFile = Path.Combine(folder, $"{filename}{GlobalConfiguration.TitanAssetMetadataFileExtension}");
        if (File.Exists(metadataFile))
        {
            Debug.WriteLine("Manifest file does already exist. This is weird :O");
            return;
        }

        await using var file = File.Create(metadataFile);
        var metadata = new TitanAssetFileMetadata { Type = TitanAssetType.Unknown };
        await JsonSerializer.SerializeAsync(file, metadata, TitanAssetFileMetadataJsonContext.Default.TitanAssetFileMetadata);
    }

    public Task OnFileRename(AssetFileRenameInfo info)
    {
        if (info.IsDirectory || IsMetadataFile(info.NewFileExtension) || IsMetadataFile(info.PreviousFileExtension))
        {
            return Task.CompletedTask;
        }

        var folder = Path.GetDirectoryName(info.NewFullPath) ?? throw new InvalidOperationException($"Failed to get the folder name from path {info.NewFullPath}");
        var oldFilename = Path.GetFileNameWithoutExtension(info.PreviousName);
        var newFilename = Path.GetFileNameWithoutExtension(info.NewName);
        var oldMetadataFilePath = Path.Combine(folder, $"{oldFilename}{GlobalConfiguration.TitanAssetMetadataFileExtension}");
        var newMetadataFilePath = Path.Combine(folder, $"{newFilename}{GlobalConfiguration.TitanAssetMetadataFileExtension}");

        if (!File.Exists(oldMetadataFilePath))
        {
            Debug.WriteLine("old metadata file does not exist.");
            return Task.CompletedTask;
        }

        if (File.Exists(newMetadataFilePath))
        {
            Debug.WriteLine("New file alreayd exists. Wth?");
            return Task.CompletedTask;
        }
        File.Move(oldMetadataFilePath, newMetadataFilePath, true);
        return Task.CompletedTask;
    }

    public Task OnFileDeleted(AssetFileDeletedInfo info)
    {
        if (IsMetadataFile(info.FileExtension))
        {
            //NOTE(Jens): this should trigger a regenration of this file
            return Task.CompletedTask;
        }
        var folder = Path.GetDirectoryName(info.FullPath) ?? throw new InvalidOperationException($"Failed to get the folder name from path {info.FullPath}");
        var filename = Path.GetFileNameWithoutExtension(info.Name);

        var metadataFilePath = Path.Combine(folder, $"{filename}{GlobalConfiguration.TitanAssetMetadataFileExtension}");
        var deletedMetadataFilePath = Path.Combine(folder, $"{filename}{GlobalConfiguration.TitanAssetMetadataFileExtension}.deleted");

        if (!File.Exists(metadataFilePath))
        {
            Debug.WriteLine("The metadata file does not exist.");
            return Task.CompletedTask;
        }

        //NOTE(Jens): If the same file is deleted multiple times it will just overwrite the old backup file
        File.Move(metadataFilePath, deletedMetadataFilePath, true);
        return Task.CompletedTask;
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(TitanAssetFileMetadata))]
internal partial class TitanAssetFileMetadataJsonContext : JsonSerializerContext { }

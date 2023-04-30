using Titan.Assets;
using Titan.Core.Logging;
using Titan.Tools.Core.Audio;
using Titan.Tools.Core.Images;
using Titan.Tools.Core.Manifests;
using Titan.Tools.Core.Models;
using Titan.Tools.Core.Models.WavefrontObj;
using Titan.Tools.Core.Shaders;

namespace Titan.Editor;

internal class RawAssetFileReader : IAssetFileReader
{
    //NOTE(Jens): Raw assets might increase or decrease in size when the data changes, so we just return a bigger size for the buffer and in the Read function we return the actual bytes needed. Increase this number if something fails. We could have a static size of ~5Mb that should cover all assets.
    private const uint FileSizeMultiplier = 2;
    private (string BasePath, object[] Assets)[] _manifests;

    public bool Init(AssetsConfiguration[] configs, AssetsDevConfiguration devConfig)
    {
        var shaderCompilerPath = Path.Combine(devConfig.EnginePath, "libs", "dxc", "bin", "x64");
        if (!Directory.Exists(shaderCompilerPath))
        {
            Logger.Error<RawAssetFileReader>("The DXC library path does not exist. make sure you've setup the Engine correctly.");
            return false;
        }
        Logger.Trace<RawAssetFileReader>($"Setting DXC dll path to: {shaderCompilerPath}.");
        ShaderCompiler.SetShaderCompilerDllFolder(shaderCompilerPath);

        var highestId = configs.Max(c => c.Id) + 1;
        _manifests = new (string BasePath, object[])[highestId];
        foreach (var config in configs)
        {
            var basePath = Path.GetDirectoryName(config.ManifestFile);
            _manifests[config.Id] = (basePath, config.RawAssets);
        }

        return true;
    }
    public ulong GetSizeFromDescriptor(in AssetDescriptor descriptor)
        => descriptor.GetSize() * FileSizeMultiplier;

    public int Read(Span<byte> buffer, in AssetDescriptor descriptor)
    {
        var (basePath, assets) = _manifests[descriptor.ManifestId];
        var asset = assets[descriptor.Id];
        return descriptor.Type switch
        {
            AssetDescriptorType.Shader => ReadAndCompileShader(basePath, (ShaderItem)asset, buffer),
            AssetDescriptorType.Audio => ReadAudio(basePath, (AudioItem)asset, buffer),
            AssetDescriptorType.Texture => ReadTexture(basePath, (TextureItem)asset, buffer),
            AssetDescriptorType.Model => ReadModel(basePath, (ModelItem)asset, buffer),
            _ => throw new NotImplementedException($"The descriptor type {descriptor.Type} has not been implemented yet.")
        };
    }

    private static int ReadModel(string basePath, ModelItem asset, Span<byte> buffer)
    {
        var path = Path.Combine(basePath, asset.Path);
        var result = ModelConverter.ReadModel(path);
        if (!result.Success)
        {
            Logger.Error<RawAssetFileReader>($"failed to read the model from path {path}, with error {result.Error}");
            return -1;
        }
        return -1;
    }

    private static int ReadAudio(string basePath, AudioItem asset, Span<byte> buffer)
    {
        var path = Path.Combine(basePath, asset.Path);
        var result = WaveReader.Read(path);
        if (!result.Success || result.Sound == null)
        {
            Logger.Error<RawAssetFileReader>($"Failed to read the audio from path {path}, with error message: {result.Error}");
            return -1;
        }
        if (result.Sound.Data.Length > buffer.Length)
        {
            Logger.Error<RawAssetFileReader>($"The buffer can't fit shader bytecode from shader {asset.Name}");
            return -1;
        }
        result.Sound.Data.CopyTo(buffer);
        return result.Sound.Data.Length;
    }

    private static int ReadAndCompileShader(string basePath, ShaderItem asset, Span<byte> buffer)
    {
        var path = Path.Combine(basePath, asset.Path);
        using var result = ShaderCompiler.Compile(path, asset.EntryPoint, asset.ShaderModel);
        if (!result.Succeeded)
        {
            Logger.Error<RawAssetFileReader>($"Failed to compile the shader at path {path}, with error message: {result.Error}");
            return -1;
        }

        var byteCode = result.GetByteCode();
        if (byteCode.Length > buffer.Length)
        {
            Logger.Error<RawAssetFileReader>($"The buffer can't fit shader bytecode from shader {asset.Name}");
            return -1;
        }
        byteCode.CopyTo(buffer);
        return byteCode.Length;
    }

    private static int ReadTexture(string basePath, TextureItem asset, Span<byte> buffer)
    {
        var path = Path.Combine(basePath, asset.Path);
        using var imageReader = new ImageReader();
        var image = imageReader.LoadImage(path);
        if (image == null)
        {
            Logger.Error<RawAssetFileReader>($"Failed to load the image file from path {path}");
            return -1;
        }
        if (image.Data.Length > buffer.Length)
        {
            Logger.Error<RawAssetFileReader>($"The buffer can't fit the texture with name {asset.Name}");
            return -1;
        }
        image.Data.CopyTo(buffer);
        return image.Data.Length;
    }
}

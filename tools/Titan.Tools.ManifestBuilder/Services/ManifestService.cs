using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.Services;

public interface IManifestService
{
    Task<Result<IReadOnlyList<Manifest>>> ListManifests(string projectPath);
    Task<Result<Manifest>> CreateManifest(string projectPath, string name);
    Task<Result> SaveManifest(string projectPath, Manifest manifest);
}

internal class ManifestService : IManifestService
{
    private readonly IJsonSerializer _jsonSerializer;

    public ManifestService(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public async Task<Result<Manifest>> CreateManifest(string projectPath, string name)
    {
        var path = GetManifestPath(projectPath, name);
        if (File.Exists(path))
        {
            return Result<Manifest>.Fail($"A manifest with the name {name} already exists in the project.");
        }
        var manifest = new Manifest
        {
            Name = name,
            Path = path
        };

        var saveResult = await SaveManifest(projectPath, manifest);
        if (saveResult.Failed)
        {
            return Result<Manifest>.Fail("Failed to save the manifest with an unknown error.");
        }
        return Result<Manifest>.Success(manifest);
    }

    public async Task<Result> SaveManifest(string projectPath, Manifest manifest)
    {
        var path = manifest.Path;
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(Manifest.Path), "Path is null");
        }

        try
        {
            await using var file = File.OpenWrite(path);
            file.SetLength(0);
            await _jsonSerializer.SerializeAsync(file, manifest, true);
        }
        catch (Exception e)
        {
            return Result.Fail($"Save failed with {e.GetType().Name} and message {e.Message}");
        }
        return Result.Success();
    }

    private static string GetManifestPath(string projectPath, string name)
    {
        return Path.Combine(projectPath, $"{GetSafeFileName(name)}.{GlobalConfiguration.ManifestFileExtension}");

        static string GetSafeFileName(string name)
            => Path
                .GetInvalidFileNameChars()
                .Concat(new[] { ' ' })
                .Aggregate(name, (current, invalidChar) => current.Replace(invalidChar, '_'))
                .ToLowerInvariant()
                .Trim();
    }


    public async Task<Result<IReadOnlyList<Manifest>>> ListManifests(string projectPath)
    {
        var paths = Directory.GetFiles(projectPath, $"*.{GlobalConfiguration.ManifestFileExtension}", SearchOption.TopDirectoryOnly);

        List<Manifest> manifests = new(paths.Length);
        foreach (var path in paths)
        {
            await using var file = File.OpenRead(path);
            var manifest = await _jsonSerializer.DeserializeAsync<Manifest>(file);
            if (manifest == null)
            {
                return Result<IReadOnlyList<Manifest>>.Fail($"Failed to deserialize contents of {path}.");
            }
            manifest.Path = path;
            manifests.Add(manifest);
        }
        return Result<IReadOnlyList<Manifest>>.Success(manifests);
    }
}

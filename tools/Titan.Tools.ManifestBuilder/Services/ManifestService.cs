using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.Models;

namespace Titan.Tools.ManifestBuilder.Services;

public interface IManifestService
{
    Task<Result<IReadOnlyList<Manifest>>> ListManifests(string projectPath);
}

internal class ManifestService : IManifestService
{
    private readonly IJsonSerializer _jsonSerializer;

    public ManifestService(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
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
            manifests.Add(manifest);
        }
        return Result<IReadOnlyList<Manifest>>.Success(manifests);
    }
}

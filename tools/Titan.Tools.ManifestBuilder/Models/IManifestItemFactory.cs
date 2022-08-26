using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.Common;

namespace Titan.Tools.ManifestBuilder.Models;

public interface IManifestItemFactory
{
    Result<ManifestItem> CreateFromPath(string relativePath);
}

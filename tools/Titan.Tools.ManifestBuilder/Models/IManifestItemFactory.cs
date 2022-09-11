using Titan.Tools.Core.Common;
using Titan.Tools.Core.Manifests;

namespace Titan.Tools.ManifestBuilder.Models;

public interface IManifestItemFactory
{
    Result<ManifestItem> CreateFromPath(string relativePath);
}

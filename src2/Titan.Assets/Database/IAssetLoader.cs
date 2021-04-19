using System;
using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Assets.Database
{
    public interface IAssetLoader : IDisposable
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<AssetDependency> dependencies);
        public void OnRelease(object asset);
    }
}

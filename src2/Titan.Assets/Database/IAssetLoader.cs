using System;
using Titan.Core.Memory;

namespace Titan.Assets.Database
{
    public interface IAssetLoader : IDisposable
    {
        public object OnLoad(in MemoryChunk<byte>[] buffers);
        public void OnRelease(object asset);
    }
}

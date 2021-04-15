using System;
using Titan.Core.Memory;

namespace Titan.Assets.Database
{
    public interface IAssetLoader : IDisposable
    {
        public string Type { get; }
        public int OnLoad(in MemoryChunk<byte>[] buffers);
        public void OnRelease(int handle);
    }
}

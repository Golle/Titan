using Titan.Core.Memory;

namespace Titan.Assets.Database
{
    public interface IAssetLoader : IDisposable
    {
        public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies);
        public void OnRelease(int handle);
    }
}

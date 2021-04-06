using Titan.Core.Memory;

namespace Titan.Assets
{
    public interface IAsset
    {
        public void OnLoad(in MemoryChunk<byte> buffer);
        public void OnRelease();
    }
}

using Titan.Assets.Database;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public struct Asset
    {
        public string Identifier;
        public string Type;
        public IAssetLoader Loader;
        public AssetStatus Status;
        public string[] Files;
        public int ReferenceCount;
        public MemoryChunk<byte>[] FileBytes;
        public int AssetHandle;
        public object AssetReference;
        public bool Static;
        public int[] Dependencies;
    }
}

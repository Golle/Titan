using Titan.Assets.Database;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public struct Asset
    {
        public string Identifier;
        public IAssetLoader Loader;
        public AssetStatus Status;
        public string File;
        public int ReferenceCount;
        public MemoryChunk<byte> FileBytes;
        public int AssetHandle;
        public bool Static;
        public string[] Dependencies;
    }
}

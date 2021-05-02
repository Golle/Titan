using Titan.Assets.Database;
using Titan.Core.Memory;

namespace Titan.Assets
{
    public struct Asset
    {
        public string Identifier;
        public AssetTypes Type;
        public IAssetLoader Loader;
        public AssetStatus Status;
        public string[] Files;
        public int ReferenceCount;
        public MemoryChunk<byte>[] FileBytes;
        public int AssetHandle;
        public object AssetReference;
        public bool Static;
        public AssetDependency[] Dependencies;
    }

    public readonly struct AssetDependency
    {
        public readonly string Name;
        public readonly int Index;
        public AssetDependency(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}

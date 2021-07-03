namespace Titan.Assets
{
    public readonly struct Dependency
    {
        public readonly AssetTypes Type;
        public readonly string Id;
        public readonly string Name;
        public readonly int AssetHandle;
        public Dependency(AssetTypes type, string id, string name, int assetHandle)
        {
            Type = type;
            Id = id;
            Name = name;
            AssetHandle = assetHandle;
        }
    }
}

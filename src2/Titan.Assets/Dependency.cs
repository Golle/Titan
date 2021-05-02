namespace Titan.Assets
{
    public readonly struct Dependency
    {
        public readonly AssetTypes Type;
        public readonly string Id;
        public readonly string Name;
        public readonly object Asset;
        public Dependency(AssetTypes type, string id, string name, object asset)
        {
            Type = type;
            Id = id;
            Name = name;
            Asset = asset;
        }
    }
}

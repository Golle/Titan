namespace Titan.Assets
{
    public readonly struct AssetDependency
    {
        public readonly AssetTypes Type;
        public readonly string Identifier;
        public readonly object Asset;
        public AssetDependency(AssetTypes type, string identifier, object asset)
        {
            Type = type;
            Identifier = identifier;
            Asset = asset;
        }
    }
}

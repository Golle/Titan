namespace Titan.Assets
{
    public readonly struct AssetDependency
    {
        public readonly string Type;
        public readonly string Identifier;
        public readonly object Asset;
        public AssetDependency(string type, string identifier, object asset)
        {
            Type = type;
            Identifier = identifier;
            Asset = asset;
        }
    }
}

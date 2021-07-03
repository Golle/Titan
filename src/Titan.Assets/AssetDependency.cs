namespace Titan.Assets
{
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

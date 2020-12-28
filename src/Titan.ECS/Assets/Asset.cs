namespace Titan.ECS.Assets
{
    public readonly struct Asset<T> where T : unmanaged
    {
        public readonly string Identifier;
        public Asset(string identifier) => Identifier = identifier;
    }
}

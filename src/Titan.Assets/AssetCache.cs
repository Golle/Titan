namespace Titan.Assets
{
    internal class AssetCache<T>
    {

        public void Add(string identifier, in T asset)
        {

        }

        public bool TryGet(string identifier, out T asset)
        {
            asset = default;
            return true;
        }
    }
}

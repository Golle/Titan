using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Assets
{
    internal struct AssetDependency
    {
        public int Handle; // TODO: what should a handle represent?
        public string Identifier;
        public string Type;
    }


    public interface IAssetLoader { }

    public struct TextureLoader1 : IAssetLoader { }

    

    public abstract class AssetLoader
    {
        protected abstract void Load();
    }







    //public abstract class AssetStorage<T>
    //{

    //    public bool Contains(string identifier)
    //    {
    //        return true;
    //    }

    //    public T Get(string identifier)
    //    {
    //        throw new InvalidOperationException();
    //    }
    //    public void Add(in Asset asset) { }
    //    public void Remove(string identifier) { }
    //}

    //public class Asset
    //{
    //}
}

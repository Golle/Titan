namespace Titan.Resources
{
    
    
    
    
    
    
    
    internal struct Asset<T, TComponent>
    {
        public T Identifier;
    }

    public class TextureAssetStorage : AssetStorage<Texture2DComponent>
    {

    }
    
    public class AssetStorage<TComponent>
    {
        public TComponent Load(string identifier)
        {
            return default;
        }
    }

    public struct Texture2DComponent
    {
        
    }
    public class TextureResourceManager  : ResourceManager<string, Texture2DComponent> { }
    
    public abstract class ResourceManager<TIdentifier, TComponent>
    {
    }
}

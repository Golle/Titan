using Titan.Graphics.Resources;

namespace Titan.Assets
{
    public interface IAssetsStorage
    {
        Handle<T> GetAsset<T>(string identifier) where T : unmanaged;
    }
}

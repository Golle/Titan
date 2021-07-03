using Titan.Core;

namespace Titan.Assets.Storage
{
    public interface IAssetStorage<T>
    {
        T Pop(in Handle<T> handle);
        Handle<T> Push(in T asset);
    }
}

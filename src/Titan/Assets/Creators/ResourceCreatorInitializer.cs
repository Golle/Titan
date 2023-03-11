using Titan.Core;
using Titan.Resources;

namespace Titan.Assets.Creators;

public readonly ref struct ResourceCreatorInitializer
{
    private readonly ResourceCollection _resourceCollection;

    internal ResourceCreatorInitializer(ResourceCollection resourceCollection)
    {
        _resourceCollection = resourceCollection;
    }

    public ObjectHandle<T> GetManagedResource<T>() where T : class
        => _resourceCollection.GetManaged<T>();
}
using Titan.Core.App;
using Titan.ECS.Systems;

namespace Titan.ECS.SystemsV2;

public abstract class ResourceSystem : ISystem
{
    protected ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged
    {
        return new ReadOnlyResource<T>();
    }
    protected MutableResource<T> GetMutableResource<T>() where T : unmanaged
    {
        return new MutableResource<T>();
    }

    protected EventsReader<T> GetEventsReader<T>() where T : unmanaged
    {
        return new();
    }
    protected EventsWriter<T> GetEventsWriter<T>() where T : unmanaged
    {
        return new();
    }

    public abstract void OnUpdate();
}

using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.Services;

public class ServiceCollection : IDisposable, IServiceCollection
{
    private readonly Dictionary<Type, object> _services = new();

    private readonly List<IServiceInit> _init = new();
    private readonly List<IServiceUpdate> _updates = new();
    private readonly List<IServicePostUpdate> _postUpdates = new();
    private readonly List<IServicePreUpdate> _preUpdates = new();
    private readonly List<IDisposable> _disposables = new();

    public ServiceCollection Register<T>(T service)
    {
        var type = typeof(T);
        if (_services.TryAdd(type, service))
        {
            Logger.Trace<ServiceCollection>($"Registered service {type.Name}");

            if (service is IServicePostUpdate postUpdate)
            {
                Logger.Trace<ServiceCollection>($"{type.Name} is {typeof(IServicePostUpdate)}");
                _postUpdates.Add(postUpdate);
            }

            if (service is IServicePreUpdate preUpdate)
            {
                Logger.Trace<ServiceCollection>($"{type.Name} is {typeof(IServicePreUpdate)}");
                _preUpdates.Add(preUpdate);
            }

            if (service is IServiceUpdate update)
            {
                Logger.Trace<ServiceCollection>($"{type.Name} is {typeof(IServiceUpdate)}");
                _updates.Add(update);
            }

            if (service is IServiceInit init)
            {
                Logger.Trace<ServiceCollection>($"{type.Name} is {typeof(IServiceInit)}");
                _init.Add(init);
            }

            if (service is IDisposable disposable)
            {
                Logger.Trace<ServiceCollection>($"{type.Name} is {typeof(IDisposable)}");
                _disposables.Add(disposable);
            }
        }
        else
        {
            Logger.Error<ServiceCollection>($"{type.Name} has already been registered.");
        }

        return this;
    }

    public T Get<T>() where T : class
    {
#if DEBUG
        return _services.TryGetValue(typeof(T), out var value) ? (T)value : throw new Exception($"Service {typeof(T)} could not be found.");
#else
        return  (T)_services[typeof(T)];
#endif
    }

    public void InitServices()
    {
        foreach (var service in _init)
        {
            service.Init(this);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PostUpdate()
    {
        foreach (var service in _postUpdates)
        {
            service.PostUpdate();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PreUpdate()
    {
        foreach (var service in _preUpdates)
        {
            service.PreUpdate();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update()
    {
        foreach (var service in _updates)
        {
            service.Update();
        }
    }

    public void Dispose()
    {
        if (_services.Count > 0)
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _services.Clear();
            _disposables.Clear();
            _preUpdates.Clear();
            _postUpdates.Clear();
            _updates.Clear();
        }
    }
}

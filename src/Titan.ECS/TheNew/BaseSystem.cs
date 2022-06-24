using System;
using System.Diagnostics;
using System.Linq;
using Titan.ECS.Systems;
using Titan.ECS.Systems.Resources;

namespace Titan.ECS.TheNew;

public abstract unsafe class BaseSystem
{
    private ISharedResources _sharedResources = new SharedResourceManager(100000);

    private uint[] _readOnly = Array.Empty<uint>();
    private uint[] _mutable = Array.Empty<uint>();

    private Type[] _dependsOn = Array.Empty<Type>();
    protected bool IsInitialized { get; private set; }

    internal virtual void Init(World_ world)
    {
        OnInit();
        IsInitialized = true;
    }

    internal virtual void Teardown(World_ world)
    {
        OnTeardown();
        IsInitialized = false;
    }

    internal void Update()
    {
        // add stats etc
        OnUpdate();
    }

    protected abstract void OnUpdate();
    protected virtual void OnTeardown(){}
    protected virtual void OnInit(){}

    protected ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged
    {
        AddReadonly<T>();
        return new(_sharedResources.GetMemoryForType<T>());
    }

    protected MutableResource<T> GetMutableResource<T>() where T : unmanaged
    {
        AddMutable<T>();
        return new(_sharedResources.GetMemoryForType<T>());
    }

    protected void AddMutable<T>()
    {
        Debug.Assert(IsInitialized == false, "System has already been initialized.");
        var resourceId = ResourceId<T>.Id;
        if (!_mutable.Contains(resourceId))
        {
            Array.Resize(ref _mutable, _mutable.Length + 1);
            _mutable[^1] = resourceId;
        }
    }
    protected void AddReadonly<T>()
    {
        Debug.Assert(IsInitialized == false, "System has already been initialized.");
        var resourceId = ResourceId<T>.Id;
        if (!_readOnly.Contains(resourceId))
        {
            Array.Resize(ref _readOnly, _readOnly.Length + 1);
            _readOnly[^1] = resourceId;
        }
    }

    protected void DependsOn<T>() where T : BaseSystem
    {
        if (!_dependsOn.Contains(typeof(T)))
        {
            Array.Resize(ref _dependsOn, _dependsOn.Length + 1);
            _dependsOn[^1] = typeof(T);
        }
    }

    internal (uint[] ReadOnly, uint[] Mutable, Type[] DependsOn) GetDependencies() => (_readOnly, _mutable, _dependsOn);
}

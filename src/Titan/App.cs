using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Titan.Core;
using Titan.Core.Events;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Systems;
using Titan.ECS.TheNew;
using Titan.NewStuff;
using Titan.Windows.D3D11;

namespace Titan;


public class EventSystem<T> : ResourceSystem where T : unmanaged
{
    private readonly MutableResource<Events<T>> _events;

    public EventSystem()
    {
        _events = GetMutableResource<Events<T>>();
    }
    public override void OnUpdate() => _events.Get().Swap();
}


public class TheWorld
{
    private ManagedResources _managedResources;
    private UnmanagedResources _unmanagedResources;

    public TheWorld()
    {

    }
    //private Dictionary<>

}

public abstract class EntitySystem : ResourceSystem
{

}

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

    protected T GetManagedResource<T>() where T : class, new()
    {
        return new T();
    }
    public abstract void OnUpdate();
}

public class App : IDisposable
{
    private readonly UnmanagedResources _resources;
    private readonly ManagedResources _managedResources;

    private readonly IPersistentMemoryAllocator _persistentMemoryAllocator;

    private readonly List<IDisposable> _disposables = new();
    public static App Create()
    {
        return new();
    }

    private App()
    {
        const int maxResourceTypes = 300;
        _persistentMemoryAllocator = new NativeMemoryAllocator(1 * 1024 * 1024 * 1024); // allocate 1GB, this should be configurable, and maybe auto adjust when needed.
        _resources = new UnmanagedResources(32 * 1024 * 1024, maxResourceTypes, _persistentMemoryAllocator);
        _managedResources = new ManagedResources(maxResourceTypes);
    }


    public App AddEvent<T>(uint maxEvents = 10) where T : unmanaged
    {
        var events = new Events<T>(maxEvents);
        return this;
    }
    public App AddResource<T>(in T resource) where T : unmanaged
    {
        _resources.InitResource(resource);
        if (resource is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
        return this;
    }

    public App AddResource<T>(T resource = null) where T : class, new()
    {
        _managedResources.InitResource(resource);
        return this;
    }
    public ref readonly T GetResource<T>() where T : unmanaged => ref _resources.GetResource<T>();
    public bool HasResource<T>() where T : unmanaged => _resources.HasResource<T>();
    public bool HasManagedResource<T>() where T : class, new() => _managedResources.HasResource<T>();
    /// <summary>
    /// Managed resource can be modified from any thread and are not thread safe (which means that systems will not look at readonly/mutable for managed types)
    /// </summary>
    /// <typeparam name="T">The managed resource type</typeparam>
    /// <returns>The instance</returns>
    public T GetManagedResource<T>() where T : class, new() => _managedResources.GetResource<T>();

    public App WithModule<T>() where T : IModule
    {
        // NOTE():this enforces the order of resources
        T.Build(this);
        return this;
    }


    public App Run()
    {

        return this;
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        _resources.Dispose();
        (_persistentMemoryAllocator as IDisposable)?.Dispose();
    }
}


internal class EventSystem : EntitySystem_
{
    public EventSystem()
    {

    }
    protected override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}

public struct WindowDescriptor : IDefault<WindowDescriptor>
{
    private const int DefaultHeight = 768;
    private const int DefaultWidth = 1024;
    private const int MaxTitleLength = 128;

    public uint Width;
    public uint Height;
    public bool Resizable;
    private unsafe fixed char _title[MaxTitleLength];
    private int _titleLength;
    public unsafe ReadOnlySpan<char> Title
    {
        readonly get
        {
            fixed (char* pTitle = _title)
            {
                return new(pTitle, _titleLength);
            }
        }
        set
        {
            _titleLength = Math.Min(MaxTitleLength, value.Length);
            fixed (char* pSource = value)
            fixed (char* pDestination = _title)
            {
                Unsafe.CopyBlock(pDestination, pSource, (uint)_titleLength * sizeof(char));
            }
        }
    }

    public static WindowDescriptor Default() => new()
    {
        Height = DefaultHeight,
        Width = DefaultWidth,
        Resizable = true,
        Title = "n/a"
    };
}

public interface IDefault<out T>
{
    static abstract T Default();
}

public struct WindowModule : IModule
{
    public static void Build(App app)
    {
        app.AddEvent<CreateWindow>();

        if (!app.HasResource<WindowDescriptor>())
        {
            app.AddResource(WindowDescriptor.Default());
        }
        var descriptor = app.GetResource<WindowDescriptor>();
        Logger.Info<WindowModule>($"Window descriptor: {descriptor.Title} - Size: {descriptor.Width}x{descriptor.Height}. Can resize: {descriptor.Resizable}");
    }
}

public struct CreateWindow
{
}
public interface IModule
{
    static abstract void Build(App app);
}

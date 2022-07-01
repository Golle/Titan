using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.SystemsV2;
[Obsolete("Delete this when the unmanaged system implementation is done.")]
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

public readonly unsafe struct MutableStorage2<T> where T : unmanaged
{
    private static readonly ComponentId ComponentId = ComponentId<T>.Id;
    private readonly Components<T>* _pool;
    private readonly EventsWriter<ComponentDestroyed>* _writer;
    public MutableStorage2(Components<T>* pool) => _pool = pool;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref _pool->Get(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _pool->Contains(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity, in T initialValue = default) => ref _pool->Create(entity, initialValue);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T CreateOrReplace(in Entity entity, in T value = default) => ref _pool->CreateOrReplace(entity, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity) => _writer->Send(new ComponentDestroyed(ComponentId, entity));


    /// <summary>
    /// This method should only be called by internal systems since it will bypass any other delete mechanic.
    /// </summary>
    /// <param name="entity"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DestroyImmediately(in Entity entity) => _pool->Destroy(entity);
}


public readonly record struct EntityDestroyed(Entity Entity);
public readonly record struct ComponentDestroyed(ComponentId Id, Entity Entity);



public struct EntityFilter
{
    // Return the entities that matches the components
    public readonly ReadOnlySpan<Entity> GetEntites() => ReadOnlySpan<Entity>.Empty;
}

// Initializer functions that will set up the resources, filters, buffers etc.
public interface ISystemsInitializer
{
    MutableResource<T> GetMutableResource<T>() where T : unmanaged;
    ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged;

    MutableStorage2<T> GetMutableStorage<T>() where T : unmanaged;
    MutableStorage2<T> GetReadOnlyStorage<T>() where T : unmanaged;

    EventsReader<T> GetEventsReader<T>() where T : unmanaged;
    EventsWriter<T> GetEventsWriter<T>() where T : unmanaged;

    EntityFilter CreateEntityFilter(EntityFilterConfiguration config);
    MemoryBlock<T> AllocateMemory<T>(int size) where T : unmanaged;
}


public interface IStructSystem<T> where T : unmanaged
{
    static abstract void Init(ref T system, ISystemsInitializer init);
    static abstract void Update(in T system);
    static bool ShouldRun(in T system) => true;
}

public struct TestSystem : IStructSystem<TestSystem>
{
    private MutableStorage2<uint> _transforms;
    private EntityFilter _filter;
    //private MemoryBlock<byte> Buffer;

    public static void Init(ref TestSystem system, ISystemsInitializer init)
    {
        //system.Transforms = init.GetMutableResource<Transform3D>();
        //system.Filter = init.CreateEntityFilter(new EntityFilterConfiguration().With<Transform3D>());
        //system.Buffer = init.AllocateMemory<byte>(100 * 1024);

        system._filter = init.CreateEntityFilter(new EntityFilterConfiguration());
        system._transforms = init.GetMutableStorage<uint>();
        
    }

    public static void Update(in TestSystem system)
    {
        //var buffer = system.Buffer.AsSpan();
        ref readonly var transforms = ref system._transforms;
        foreach (ref readonly var entity in system._filter.GetEntites())
        {
            ref var transform = ref transforms.Get(entity);
            // Do the magic
            //var length = Encoding.UTF8.GetBytes("this is a super long string that doesn't fit on the stack", buffer);

            //do something with the string, maybe update a UI element?
        }
    }
    
    //public static bool ShouldRun(in TestSystem system) => system._filter.GetEntites().Length > 0;
}

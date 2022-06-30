using System;
using System.Runtime.InteropServices;
using System.Text;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;

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

    MutableStorage<T> GetMutableStorage<T>() where T : unmanaged;
    MutableStorage<T> GetReadOnlyStorage<T>() where T : unmanaged;

    EntityFilter CreateEntityFilter(EntityFilterConfiguration config);
    MemoryBlock<T> AllocateMemory<T>(int size) where T : unmanaged;
}

static unsafe class SystemHelpers<T> where T : unmanaged, IStructSystem<T>
{
    public static void Init(void* system, ISystemsInitializer init) => T.Init(ref *(T*)system, init);
    public static void Update(void* system) => T.Update(*(T*)system);
}

public interface IStructSystem<T> where T : unmanaged
{
    static abstract void Init(ref T system, ISystemsInitializer init);
    static abstract void Update(in T system);
}

public struct TestSystem : IStructSystem<TestSystem>
{
    //private MutableResource<Transform3D> Transforms;
    //private EntityFilter Filter;
    //private MemoryBlock<byte> Buffer;


    private int A;
    private int B;
    private int C;
    public static void Init(ref TestSystem system, ISystemsInitializer init)
    {
        //system.Transforms = init.GetMutableResource<Transform3D>();
        //system.Filter = init.CreateEntityFilter(new EntityFilterConfiguration().With<Transform3D>());
        //system.Buffer = init.AllocateMemory<byte>(100 * 1024);

        system.A = 10;
        system.B = 32;
        system.C = int.MaxValue;
    }

    public static void Update(in TestSystem system)
    {
        //var buffer = system.Buffer.AsSpan();
        //ref var transform = ref system.Transforms.Get();
        //foreach (ref readonly var entity in system.Filter.GetEntites())
        //{
        //    // Do the magic
        //    var length = Encoding.UTF8.GetBytes("this is a super long string that doesn't fit on the stack", buffer);

        //    //do something with the string, maybe update a UI element?
        //}
    }
}

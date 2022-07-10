using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.SystemsV2;

public readonly unsafe struct MutableStorage2<T> where T : unmanaged, IComponent
{
    private static readonly ComponentId ComponentId = ComponentId<T>.Id;
    private readonly Components<T>* _pool;
    private readonly EventsWriter<ComponentDestroyed> _writer;
    public MutableStorage2(Components<T>* pool, EventsWriter<ComponentDestroyed> writer)
    {
        _pool = pool;
        _writer = writer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref _pool->Get(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _pool->Contains(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity, in T initialValue = default) => ref _pool->Create(entity, initialValue);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T CreateOrReplace(in Entity entity, in T value = default) => ref _pool->CreateOrReplace(entity, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity) => _writer.Send(new ComponentDestroyed(ComponentId, entity));


    /// <summary>
    /// This method should only be called by internal systems since it will bypass any other delete mechanic.
    /// </summary>
    /// <param name="entity"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DestroyImmediately(in Entity entity) => _pool->Destroy(entity);
}

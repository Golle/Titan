//NOTE(Jens): Enable this for trace logs when components are created/removed
//#define COMPONENT_POOL_TRACE

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory.Allocators;
using Titan.ECS.Entities;

namespace Titan.ECS.Components;

internal record struct PoolConfig(uint Stride, uint Count, uint MaxEntities, string Type);
file unsafe struct ComponentPoolFunctionWrapper<T> where T : unmanaged, IComponentPool
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Init(void* pool, ILinearAllocator allocator, in PoolConfig config) => ((T*)pool)->Init(allocator, config);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Create(void* pool, in Entity entity)
        => ((T*)pool)->Create(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Destroy(void* pool, in Entity entity)
        => ((T*)pool)->Destroy(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Access(void* pool, in Entity entity)
        => ((T*)pool)->Access(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(void* pool, in Entity entity)
        => ((T*)pool)->Contains(entity);
}

internal unsafe struct ComponentPool
{
    private void* _context;
    private delegate*<void*, ILinearAllocator, in PoolConfig, bool> _init;

    private delegate*<void*, in Entity, void*> _create;
    private delegate*<void*, in Entity, void> _destroy;
    private delegate*<void*, in Entity, void*> _access;
    private delegate*<void*, in Entity, bool> _contains;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Create<T>(in Entity entity) where T : unmanaged, IComponent
    {
        Trace($"Create {typeof(T).Name} for Entity {entity.Id}");
        return (T*)_create(_context, entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity)
    {
        Trace($"Destroy component for Entity {entity.Id}");
        _destroy(_context, entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* Access<T>(in Entity entity) where T : unmanaged, IComponent
    {
        Trace($"Access component  {typeof(T).Name} for Entity {entity.Id}");
        return (T*)_access(_context, entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _contains(_context,entity);

    public static ComponentPool CreatePool<T>(void* context) where T : unmanaged, IComponentPool =>
        new()
        {
            _context = context,
            _init = &ComponentPoolFunctionWrapper<T>.Init,
            _destroy = &ComponentPoolFunctionWrapper<T>.Destroy,
            _create = &ComponentPoolFunctionWrapper<T>.Create,
            _access = &ComponentPoolFunctionWrapper<T>.Access,
            _contains = &ComponentPoolFunctionWrapper<T>.Contains
        };

    public bool Init(ILinearAllocator allocator, in PoolConfig config) => _init(_context, allocator, config);

    [Conditional("COMPONENT_POOL_TRACE")]
    private static void Trace(string message) => Logger.Trace<ComponentPool>(message);
}

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Entities;

namespace Titan.ECS;

public readonly struct EntityManager
{
    private readonly ObjectHandle<EntityRegistry> _registry;
    internal EntityManager(ObjectHandle<EntityRegistry> registry)
    {
        _registry = registry;
    }

    public Entity Create()
    {
        var entity = _registry.Value.Create();
        Debug.Assert(entity.IsValid, "Failed to create a new entity.");
        return entity;
    }

    public void Destroy(Entity entity)
    {
        Debug.Assert(entity.IsValid, "Trying to destroy an entity which is not valid.");
        _registry.Value.BeginDestroy(entity);
    }

    public void Destroy(ref Entity entity)
    {
        Debug.Assert(entity.IsValid, "Trying to destroy an entity which is not valid.");
        _registry.Value.BeginDestroy(entity);
        entity = Entity.Invalid;
    }

    public void Attach(in Entity parent, in Entity child)
    {
        Debug.Assert(parent.IsValid);
        Debug.Assert(child.IsValid);
        _registry.Value.Attach(parent, child);
    }

    public void Detach(in Entity child)
    {
        Debug.Assert(child.IsValid);
        _registry.Value.Detach(child);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasParent(in Entity entity)
    {
        Debug.Assert(entity.IsValid);
        return _registry.Value.HasParent(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetParent(in Entity entity, out Entity parent)
    {
        Debug.Assert(entity.IsValid);
        return _registry.Value.TryGetParent(entity, out parent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity GetParent(in Entity entity)
    {
        Debug.Assert(entity.IsValid);
        return _registry.Value.GetParent(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetChildren(in Entity entity, Span<Entity> childrenOut)
    {
        Debug.Assert(entity.IsValid);
        return _registry.Value.GetChildren(entity, childrenOut);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity CreateChild(in Entity parent)
    {
        var entity = Create();
        Attach(parent, entity);
        return entity;
    }

    

    [Conditional("DEBUG")]
    public void DebugPrint(in Entity entity) => _registry.Value.DebugPrint(entity);
}

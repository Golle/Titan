using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.ECS.Entities;

[SkipLocalsInit]
[DebuggerDisplay("Entity={Id}")]
public readonly struct Entity
{
    public static readonly Entity Null = new();
    // NOTE(Jens): We can pack the EntityID into 24 bits and use 8 bits for any other data (world id or something else). 24 bits gives us 16777216 entities.
    public readonly uint Id;
    public Entity(uint id)
    {
        Id = id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(in Entity entity) => entity.Id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Entity(uint entityId) => new(entityId);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public void Attach(in Entity entity) => throw new NotImplementedException();// => World.GetWorldById(WorldId).AttachEntity(this, entity);
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public void Detach() => throw new NotImplementedException();//=> World.GetWorldById(WorldId).DetachEntity(this);
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public bool IsNull() => Id == 0u;
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public void Destroy() => throw new NotImplementedException();// => World.GetWorldById(WorldId).DestroyEntity(this);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public Entity CreateChildEntity() => throw new NotImplementedException();//
    ////{
    ////    var entity = World.GetWorldById(WorldId).CreateEntity();
    ////    Attach(entity);
    ////    return entity;
    ////}

    //[Conditional("DEBUG")]
    //public void DebugPrintEntityRelationship() => throw new NotImplementedException();// => World.GetWorldById(WorldId).Manager.DebugPrint(this);
}

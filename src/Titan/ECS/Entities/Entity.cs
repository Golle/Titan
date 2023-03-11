using System.Runtime.CompilerServices;

namespace Titan.ECS.Entities;

[SkipLocalsInit]
#pragma warning disable CS0660, CS0661
public readonly struct Entity
#pragma warning restore CS0660, CS0661
{
    //NOTE(Jens): We currently support up to 4 294 967 294 entities + 1 invalid. 
    //NOTE(Jens): We could use the top 8 bits (256 values) for a revision of the entity, and the lower 24 bits for the entity ID. This would require more calculations when accessing components, but would prevent invalid re-use of entity IDs
    //NOTE(Jens): Revision could be a DEBUG only feature, and disabled in release builds .
    public readonly uint Id;
    public static readonly Entity Invalid = 0;
    public bool IsValid => Id != Invalid;
    public bool IsInvalid => Id == Invalid;
    internal Entity(uint id) => Id = id;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(Entity entity) => entity.Id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Entity(uint id) => new(id);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in Entity lhs, in Entity rhs) => lhs.Id == rhs.Id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in Entity lhs, in Entity rhs) => lhs.Id != rhs.Id;


#if DEBUG
    public override string ToString() => Id.ToString();
#endif
}

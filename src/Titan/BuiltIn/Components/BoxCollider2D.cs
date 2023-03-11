using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Maths;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.Setup;

namespace Titan.BuiltIn.Components;

[StructLayout(LayoutKind.Sequential, Size = 2)]
#pragma warning disable CS0660, CS0661
public struct CollisionMask
#pragma warning restore CS0660, CS0661
{
    private ushort _value;
    public CollisionMask(ushort bitmask) => _value = bitmask;
    public static CollisionMask All => new() { _value = ushort.MaxValue };
    public static CollisionMask None => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsMatch(in CollisionMask mask) => (_value & mask._value) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort operator &(in CollisionMask lhs, in CollisionMask rhs) => (ushort)(lhs._value & rhs._value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollisionMask From(ushort mask) => new() { _value = mask };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CollisionMask(ushort mask) => From(mask);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CollisionMask(short mask) => From(unchecked((ushort)mask));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CollisionMask(int mask)
    {
        Debug.Assert((mask | ushort.MaxValue) == ushort.MaxValue, $"The size of the mask is bigger than max allowed. Max = {ushort.MaxValue}, Mask = {mask}");
        return From(unchecked((ushort)mask));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(in CollisionMask mask) => mask._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(in CollisionMask mask) => mask._value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in CollisionMask mask, int rhs) => mask._value == rhs;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in CollisionMask mask, int rhs) => mask._value != rhs;
}

public unsafe struct BoxCollider2D : IComponent, IDefault<BoxCollider2D>
{
    internal const int MaxOverlappingEntityCount = 8;
    public SizeF Size;
    public Vector2 Pivot;

    internal Vector2 BottomLeft;
    internal Vector2 TopRight;

    //NOTE(Jens): The order of these matters, they are used in the CanCollider function
    public CollisionMask Category;
    public CollisionMask CollidesWith;

    internal int OverlapCount;
    internal fixed uint Entities[MaxOverlappingEntityCount];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly ReadOnlySpan<Entity> GetOverlappingEntities()
    {
        fixed (uint* pEntity = Entities)
        {
            return new ReadOnlySpan<Entity>(pEntity, OverlapCount);
        }
    }

    public static BoxCollider2D Default => new()
    {
        Size = default,
        Pivot = Vector2.Zero
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool CanCollide(in BoxCollider2D collider)
    {
        var innerMask = *(uint*)Unsafe.AsPointer(ref Category);
        var outerMask = collider.CollidesWith | (collider.Category << 16);
        return (innerMask & outerMask) != 0;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public bool CanCollideNaive(in BoxCollider2D collider)
    //{
    //    return (Category & collider.CollidesWith) != 0 || (CollidesWith & collider.Category) != 0;
    //}
}


using System;
using System.Runtime.CompilerServices;

namespace Titan.ECS.TheNew;

public readonly struct ResourceId
{
    private readonly uint _id;
    public ResourceId(uint id) => _id = id;
    public static ResourceId Id<T>() => ResourceId<T>.Id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ResourceId l, in ResourceId r) => l._id == r._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ResourceId l, in ResourceId r) => l._id != r._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(in ResourceId resourceId) => resourceId._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(in ResourceId resourceId) => unchecked((int)resourceId._id);
    public override bool Equals(object obj) => throw new NotSupportedException("Use == to avoid boxing");
    public override int GetHashCode() => (int)_id;
    public override string ToString() => _id.ToString();
}

internal readonly struct ResourceId<T>
{
    public static readonly ResourceId Id = new(IdGenerator<ResourceId>.Next());
}


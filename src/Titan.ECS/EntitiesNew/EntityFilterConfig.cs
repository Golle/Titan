using System;
using Titan.Core;
using Titan.ECS.Components;

namespace Titan.ECS.EntitiesNew;

public readonly struct EntityFilterConfig
{
    internal readonly ComponentId Include;
    internal readonly ComponentId Exclude;
    private EntityFilterConfig(in ComponentId include, in ComponentId exclude)
    {
        Include = include;
        Exclude = exclude;
    }
    public EntityFilterConfig With<T>() where T : unmanaged, IComponent
        => new(Include | ComponentId<T>.Id, Exclude);

    public EntityFilterConfig Not<T>() where T : unmanaged, IComponent
        => new(Include, Exclude | ComponentId<T>.Id);

    public static bool operator ==(in EntityFilterConfig l, in EntityFilterConfig r) => l.Include == r.Include && l.Exclude == r.Exclude;
    public static bool operator !=(in EntityFilterConfig l, in EntityFilterConfig r) => l.Include != r.Include || l.Exclude != r.Exclude;

    public override int GetHashCode() => throw new NotSupportedException($"{nameof(GetHashCode)} should not be used for type {nameof(EntityFilterConfig)}.");
    public override bool Equals(object obj) => throw new NotSupportedException($"{nameof(Equals)} should not be used for type {nameof(EntityFilterConfig)}.");
}

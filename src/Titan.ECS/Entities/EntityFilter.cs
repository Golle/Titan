using System.Runtime.CompilerServices;

namespace Titan.ECS.Entities;

public readonly unsafe struct EntityFilter
{
    private readonly Entity* _entities;
    private readonly int* _count;
    internal EntityFilter(Entity* entities, int* count)
    {
        _entities = entities;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<Entity> GetEntities() => new(_entities, *_count);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntities() => *_count > 0;
}

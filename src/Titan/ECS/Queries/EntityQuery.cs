using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.ECS.Entities;

namespace Titan.ECS.Queries;

public readonly unsafe struct EntityQuery
{
    private readonly Entity* _entities;
    private readonly int* _count;

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *_count;
    }
    public EntityQuery(Entity* entities, int* count)
    {
        _entities = entities;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(_entities, _count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEntities() => *_count > 0;

    public ref struct Enumerator
    {
        private readonly Entity* _entities;
        private readonly int* _count;
        private int _index;
        internal Enumerator(Entity* entities, int* count)
        {
            _entities = entities;
            _count = count;
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_index < *_count;

        public readonly ref readonly Entity Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _entities[_index];
        }
    }

    public ref readonly Entity this[uint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(index < *_count, "Index is out of bounds.");
            return ref _entities[index];
        }
    }
}

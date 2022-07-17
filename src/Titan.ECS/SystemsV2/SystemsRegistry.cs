using System;
using Titan.Core.Memory;

namespace Titan.ECS.SystemsV2;

internal unsafe struct SystemsRegistry
{
    private SystemDescriptor* _systems;
    private int _count;

    public void Init(in MemoryPool pool, ReadOnlySpan<SystemDescriptor> systems)
    {
        _count = systems.Length;
        _systems = pool.GetPointer<SystemDescriptor>((uint)_count);
        systems.CopyTo(new Span<SystemDescriptor>(_systems, _count));
    }
    public readonly ReadOnlySpan<SystemDescriptor> GetDescriptors() => new(_systems, _count);
}

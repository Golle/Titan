using System;
using Titan.Memory;

namespace Titan.ECS.Systems;

internal unsafe struct SystemsRegistry
{
    private SystemDescriptor* _systems;
    private int _count;

    public void Init(in PlatformAllocator allocator, ReadOnlySpan<SystemDescriptor> systems)
    {
        _count = systems.Length;
        _systems = allocator.Allocate<SystemDescriptor>((uint)_count);
        systems.CopyTo(new Span<SystemDescriptor>(_systems, _count));
    }
    public readonly ReadOnlySpan<SystemDescriptor> GetDescriptors() => new(_systems, _count);
}

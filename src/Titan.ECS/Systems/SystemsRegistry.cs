using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Memory;

namespace Titan.ECS.Systems;

internal unsafe struct SystemsRegistry
{
    private TitanArray<SystemDescriptor> _systems;
    private int _count;

    public void Init(MemoryManager* memoryManager, ReadOnlySpan<SystemDescriptor> systems)
    {
        _count = systems.Length;
        _systems = memoryManager->AllocArray<SystemDescriptor>((uint)_count);
        systems.CopyTo(new Span<SystemDescriptor>(_systems, _count));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<SystemDescriptor> GetDescriptors() => _systems;
}

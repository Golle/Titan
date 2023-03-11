using Titan.Systems;

namespace Titan.Memory;

internal struct TemporaryMemorySystem : ISystem
{
    private MutableResource<PerFrameArena> _allocator;
    public void Init(in SystemInitializer init)
    {
        _allocator = init.GetMutableResource<PerFrameArena>();
    }

    public void Update() => _allocator.Get().Reset();
}

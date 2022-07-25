namespace Titan.Memory;

/// <summary>
/// Allocator with a context and creation arguments. Use this when you've got a state, for example when you allocate memory upfront.
/// </summary>
/// <typeparam name="TArguments"></typeparam>
public unsafe interface IAllocator<TArguments> where TArguments : unmanaged
{
    static abstract void* CreateContext(in TArguments args);
    static abstract void ReleaseContext(void* context);
    static abstract void* Allocate(void* context, nuint size);
    static abstract void Free(void* context, void* ptr);
}

/// <summary>
/// Allocated without a context. use this when you don't need to store any information about the allocations.
/// </summary>
public unsafe interface IAllocator
{
    static abstract void* Allocate(nuint size);
    static abstract void Free(void* ptr);
}

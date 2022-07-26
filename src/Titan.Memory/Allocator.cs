using System.Runtime.CompilerServices;

namespace Titan.Memory;

public unsafe struct Allocator
{
    private void* _context;
    private delegate*<void*, nuint, void*> _allocate;
    private delegate*<void*, void*, void> _free;
    private delegate*<void*, void> _release;

    /// <summary>
    /// Allocate a memory block with a size
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void* Allocate(nuint size) => _allocate(_context, size);

    /// <summary>
    /// Free the memory pointer allocated with this allocator
    /// </summary>
    /// <param name="ptr"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Free(void* ptr) => _free(_context, ptr);

    /// <summary>
    /// Releases the context
    /// </summary>
    public void Release()
    {
        if (_release != null)
        {
            _release(_context);
        }
    }
    /// <summary>
    /// Create an allocator without a context
    /// </summary>
    /// <typeparam name="T">The allocator type</typeparam>
    /// <returns>The allocator</returns>
    public static Allocator Create<T>() where T : unmanaged, IAllocator =>
        new()
        {
            _allocate = &FunctionWrapper<T>.Allocate,
            _free = &FunctionWrapper<T>.Free,
            _context = null,
            _release = null
        };

    /// <summary>
    /// Create an allocator with a context and arguments
    /// </summary>
    /// <typeparam name="TAllocator">The allocator type</typeparam>
    /// <typeparam name="TArguments">The argument type for the allocator</typeparam>
    /// <param name="args">The arguments</param>
    /// <returns>The allocator</returns>
    public static Allocator Create<TAllocator, TArguments>(in TArguments args)
        where TAllocator : unmanaged, IAllocator<TArguments>
        where TArguments : unmanaged =>
        new()
        {
            _context = TAllocator.CreateContext(args),
            _release = &TAllocator.ReleaseContext,
            _allocate = &TAllocator.Allocate,
            _free = &TAllocator.Free
        };

    //NOTE(Jens): Use this so we can use the same function pointer structure as the one with a context
    private struct FunctionWrapper<T> where T : unmanaged, IAllocator
    {
        public static void* Allocate(void* _, nuint size) => T.Allocate(size);
        public static void Free(void* _, void* ptr) => T.Free(ptr);
    }
}

using System.Runtime.CompilerServices;

namespace Titan.Core;

public unsafe struct Ref<T> where T : unmanaged
{
    //NOTE(Jens): We can explore using this as a wrapper around unmanaged resources. This would make it easier IF we want to put everything in unmanaged memory
    private T* _ptr;
    public Ref(T* ptr) => _ptr = ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AsRef() => ref *_ptr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* AsPointer() => _ptr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(in Ref<T> r) => r._ptr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Ref<T>(T* ptr) => new() { _ptr = ptr };
}
